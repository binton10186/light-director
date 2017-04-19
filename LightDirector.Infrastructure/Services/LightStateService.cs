using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Services
{
   public class LightStateService : ILightStateService
   {
      private readonly ICueTimeService _cueTimeService;
      private readonly IStagePositionService _stagePositionService;
      private readonly IEventAggregator _eventAggregator;

      private readonly Dictionary<Guid, int> _brightnesses = new Dictionary<Guid, int>();
      private readonly Dictionary<Guid, Color> _colors = new Dictionary<Guid, Color>();
      private readonly Dictionary<Guid, Vector3D> _directions = new Dictionary<Guid, Vector3D>();
      private readonly Dictionary<Guid, Dictionary<int, int>> _channelValues = new Dictionary<Guid, Dictionary<int, int>>();

      private readonly LightSpecificationRepository _lightSpecificationRepository;
      private readonly LightChannelValueChangedEvent _lightChannelValueChangedEvent;
      private readonly LightBrightnessChangedEvent _lightBrightnessChangedEvent;
      private readonly LightDirectionChangedEvent _lightDirectionChangedEvent;
      private readonly LightColorChangedEvent _lightColorChangedEvent;

      private LightingPlan _lightingPlan;
      private ICue _currentCue;
      private ILightingPlanService _lightingPlanService;

      public LightStateService(
         ICueTimeService cueTimeService, 
         IStagePositionService stagePositionService, 
         LightSpecificationRepository lightSpecificationRepository, 
         IEventAggregator eventAggregator,
         ILightingPlanService lightingPlanService)
      {
         _cueTimeService = cueTimeService;
         _cueTimeService.TimeChanged += OnCueTimeChanged;
         _stagePositionService = stagePositionService;
         _lightSpecificationRepository = lightSpecificationRepository;
         _eventAggregator = eventAggregator;
         _lightingPlanService = lightingPlanService;

         _lightingPlan = _lightingPlanService.GetLightingPlan();

         _lightChannelValueChangedEvent = _eventAggregator.GetEvent<LightChannelValueChangedEvent>();
         _lightBrightnessChangedEvent = _eventAggregator.GetEvent<LightBrightnessChangedEvent>();
         _lightColorChangedEvent = _eventAggregator.GetEvent<LightColorChangedEvent>();
         _lightDirectionChangedEvent = _eventAggregator.GetEvent<LightDirectionChangedEvent>();

         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(OnLightingPlanChanged);
         _eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);
      }

      private void OnLightingPlanChanged(LightingPlan lightingPlan)
      {
         if (_lightingPlan != null)
            _lightingPlan.LightsChanged -= OnCueKeyframesChanged;

         _lightingPlan = lightingPlan;

         if (_lightingPlan != null)
            _lightingPlan.LightsChanged += OnCueKeyframesChanged;
      }

      private void OnSelectedCueChanged(ICue cue)
      {
         if (cue == null)
         {
            if (_currentCue != null)
               _currentCue.KeyFramesChanged -= OnCueKeyframesChanged;
            _currentCue = null;

            foreach (var light in _lightingPlan?.Lights)
               RaiseBrightnessChanged(light.Id, 0);
         }
         else
         {
            var lightingPlan = _lightingPlan;
            var fadeTime = TimeSpan.FromSeconds(cue.FadeInSeconds);

            if (_currentCue != null && cue.FadeInSeconds > 0)
               cue = _currentCue.CreateFadeInCue(_cueTimeService.Time, fadeTime, cue, lightingPlan.Lights, _stagePositionService, _lightSpecificationRepository);

            _currentCue = cue;
            RaiseLightsChanged(TimeSpan.Zero, cue);
            cue.KeyFramesChanged += OnCueKeyframesChanged;
         }
      }

      private void OnCueKeyframesChanged(object sender, EventArgs e)
      {
         var currentCue = _currentCue;
         if (currentCue != null)
            RaiseLightsChanged(_cueTimeService.Time, currentCue);
      }

      private void OnCueTimeChanged(object sender, CueTimeTickEventArgs e)
      {
         var cue = _currentCue;
         if (cue == null)
            return;

         RaiseLightsChanged(e.CueTimeElapsed, cue);
      }

      private void RaiseLightsChanged(TimeSpan cueTime, ICue cue)
      {
         var lightingPlan = _lightingPlan;
         if (lightingPlan == null)
            return;

         var allLights = lightingPlan.Lights.ToArray();

          var cueAtTime = cue.GetCueAt(cueTime, allLights, _stagePositionService, _lightSpecificationRepository);
         foreach (var setting in cueAtTime.ParCanSettings)
         {
            RaiseBrightnessChanged(setting.Id, setting.Brightness);
            RaiseColorChanged(setting.Id, setting.Color);
            RaiseDirectionChanged(setting.Id, setting.Direction);

            Dictionary<int, int> channelValues;
            if(!_channelValues.TryGetValue(setting.Id, out channelValues))
            {
               channelValues = new Dictionary<int, int>();
               _channelValues[setting.Id] = channelValues;
            }

            foreach(var channelValue in setting.ChannelValues)
            {
               int originalChannelValue;
               if(!channelValues.TryGetValue(channelValue.ChannelId, out originalChannelValue) || originalChannelValue != channelValue.Value)
               {
                  _lightChannelValueChangedEvent
                     .Publish(new DmxChannelValue(setting.Id, channelValue.ChannelId, channelValue.Value));
                  channelValues[channelValue.ChannelId] = channelValue.Value;
               }
               
            }
         }

         var lightIdsForCurrentCue = cueAtTime.ParCanSettings.Select(s => s.Id);
         var lightsWithoutSettings = allLights.Where(l => !lightIdsForCurrentCue.Contains(l.Id));

         foreach(var light in lightsWithoutSettings)
            RaiseBrightnessChanged(light.Id, 0);
      }
      
      private void RaiseBrightnessChanged(Guid lightId, int brightness)
      {
         int originalBrightness;
         if (!_brightnesses.TryGetValue(lightId, out originalBrightness) || originalBrightness != brightness)
         {
            _lightBrightnessChangedEvent.Publish(new BrightnessEventArgs(lightId, brightness));
            _brightnesses[lightId] = brightness;
         }         
      }

      private void RaiseColorChanged(Guid lightId, Color color)
      {
         Color originalColor;
         if (!_colors.TryGetValue(lightId, out originalColor) || originalColor != color)
         {
            _lightColorChangedEvent
               .Publish(new ColorEventArgs(lightId, color));
            _colors[lightId] = color;
         }
      }

      private void RaiseDirectionChanged(Guid lightId, Vector3D direction)
      {
         Vector3D originalDirection;
         if (!_directions.TryGetValue(lightId, out originalDirection) || originalDirection != direction)
         {
            _lightDirectionChangedEvent
               .Publish(new DirectionEventArgs(lightId, direction));
            _directions[lightId] = direction;
         }
      }
   }
}
