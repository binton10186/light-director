using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Services;
using LightDirector.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using LightDirector.Domain;
using LightDirector.Infrastructure.ViewModels;

namespace LightDirector.ViewModels
{
   public class TheatreModel
   {
      private ModelVisual3D _scene;
      private readonly Dictionary<Guid, LightState> _lightStates = new Dictionary<Guid, LightState>();
      private readonly Dictionary<Guid, SpotLight> _lights = new Dictionary<Guid, SpotLight>();
      private readonly AmbientLight _ambientLight = new AmbientLight();
      private readonly IModeService _modeService;
      private readonly LightSpecificationRepository _lightSpecificationRepository;
      private readonly ILightingPlanService _lightingPlanService;
      private IEventAggregator _eventAggregator;

      public event Action ModelChanged;

      public TheatreModel( 
         IModeService modeService, 
         LightSpecificationRepository lightSpecificationRepository, 
         ILightingPlanService lightingPlanService,
         IEventAggregator eventAggregator)
      {
         _eventAggregator = eventAggregator;
         _eventAggregator.GetEvent<LightAddedEvent>().Subscribe(LightAdded, ThreadOption.PublisherThread);
         _eventAggregator.GetEvent<LightPositionChangedEvent>().Subscribe(LightPositionChanged, ThreadOption.PublisherThread);
         _eventAggregator.GetEvent<LightBrightnessChangedEvent>().Subscribe(LightBrightnessChanged, ThreadOption.PublisherThread);
         _eventAggregator.GetEvent<LightColorChangedEvent>().Subscribe(LightColorChanged, ThreadOption.PublisherThread);
         _eventAggregator.GetEvent<LightDirectionChangedEvent>().Subscribe(LightDirectionChanged, ThreadOption.PublisherThread);
         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(LightingPlanChanged);
         
         _modeService = modeService;
         _lightSpecificationRepository = lightSpecificationRepository;

         _lightingPlanService = lightingPlanService;
      }

      private void LightingPlanChanged(LightingPlan obj)
      {
         //var spotLights = _scene.Children.OfType<ModelVisual3D>().Where(c => c.Content is SpotLight).ToArray();
         //foreach (var spotLight in spotLights)
         //   _scene.Children.Remove(spotLight);

         //_lightStates.Clear();
         //_lights.Clear();
      }

      public bool HasScene { get { return _scene != null; } }

      public ModelVisual3D GetModel()
      {
         return _scene;
      }

      public void SetScene(ModelVisual3D scene, ScreensModel screensModel)
      {
         _scene = scene;
         _scene.Children.Add(new ModelVisual3D { Content = _ambientLight });
         _scene.Children.Add(screensModel.Left.Model);
         _scene.Children.Add(screensModel.Center.Model);
         _scene.Children.Add(screensModel.Right.Model);

         RaiseModelChanged();
      }

      private void LightAdded(Domain.Light light)
      {
         //var color = Colors.PaleGoldenrod;
         //var modelColor = GetModelColor(color, 0);
         
         //var spotLight = new SpotLight(modelColor, light.Position, light.Direction, 45, 60);
         //_scene.Children.Add(new ModelVisual3D { Content = spotLight });
         //_lightStates[light.Id] = new LightState(color, 0, light.Direction);
         //_lights[light.Id] = spotLight;
      }

      public void LightColorChanged(ColorEventArgs e)
      {
         if (_modeService.IsLive)
            return;

         LightState lightState;
         if (!_lightStates.TryGetValue(e.LightId, out lightState))
            return;

         var light = _lights[e.LightId];
         lightState = new LightState(e.Color, lightState.Brightness, lightState.Direction);
         _lightStates[e.LightId] = lightState;
         light.Color = GetModelColor(lightState.Color, lightState.Brightness);
      }

      private void LightBrightnessChanged(BrightnessEventArgs e)
      {
         if (_modeService.IsLive)
            return;

         LightState lightState;
         if (!_lightStates.TryGetValue(e.LightId, out lightState))
            return;

         var planLight = _lightingPlanService.GetLightingPlan().Lights.SingleOrDefault(l => l.Id == e.LightId);
         var specification = _lightSpecificationRepository.RetrieveById(planLight.SpecificationId);

         var brightness = e.Brightness;

         if (!specification.HasBrightnessControl)
            brightness = 100;

         var light = _lights[e.LightId];
         lightState = new LightState(lightState.Color, brightness, lightState.Direction);
         _lightStates[e.LightId] = lightState;
         light.Color = GetModelColor(lightState.Color, lightState.Brightness);
      }

      public void LightDirectionChanged(DirectionEventArgs e)
      {
         if (_modeService.IsLive)
            return;

         LightState lightState;
         if (!_lightStates.TryGetValue(e.LightId, out lightState))
            return;

         var light = _lights[e.LightId];
         lightState = new LightState(lightState.Color, lightState.Brightness, e.Direction);
         _lightStates[e.LightId] = lightState;
         light.Direction = e.Direction;
      }

      private void LightPositionChanged(LightPositionChangedEventArgs args)
      {
         //var light = _lights[args.LightId];
         //light.Position = args.Position;
      }

      public void SetAmbientLight(bool isOn)
      {
         _ambientLight.Color = isOn ? Colors.White : Colors.Black;
      }

      private Color GetModelColor(Color color, int brightness)
      {
         var red = Convert.ToByte(color.R / 100.0 * brightness);
         var green = Convert.ToByte(color.G / 100.0 * brightness);
         var blue = Convert.ToByte(color.B / 100.0 * brightness);
         color = Color.FromRgb(red, green, blue);
         return color;
      }

      private void RaiseModelChanged()
      {
         var evt = ModelChanged;
         if (evt != null)
            evt();
      }
   }
}