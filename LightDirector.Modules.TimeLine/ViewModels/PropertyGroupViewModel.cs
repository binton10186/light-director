using System;
using System.Collections.ObjectModel;
using LightDirector.Domain;
using System.Linq;
using LightDirector.Services;
using Autofac;
using System.Collections.Generic;
using LightDirector.Infrastructure.Domain;

namespace LightDirector.ViewModels
{
   public class PropertyGroupViewModel
   {
      private readonly Domain.Light _light;
      private readonly ICue _cue;
      private readonly Func<Keyframe, KeyframeViewModel> _keyFrameViewModelFactory;
      private readonly LightSpecification _lightSpecification;
      private readonly ILifetimeScope _lifetimeScope;

      public PropertyGroupViewModel(
         ICue cue, 
         Domain.Light light, 
         LightSpecificationRepository lightSpecificationRepository, 
         Func<Keyframe, KeyframeViewModel> keyframeViewModelFactory,
         ILifetimeScope lifetimeScope)
      {
         _lifetimeScope = lifetimeScope;
         _light = light;
         _cue = cue;
         _lightSpecification = lightSpecificationRepository.RetrieveById(_light.SpecificationId);         

         _cue.KeyFramesChanged += OnKeyframesChanged;

         _keyFrameViewModelFactory = keyframeViewModelFactory;

         BrightnessKeyframes = new ObservableCollection<KeyframeViewModel>();
         ColorKeyframes = new ObservableCollection<KeyframeViewModel>();
         DirectionKeyframes = new ObservableCollection<KeyframeViewModel>();
         ChannelKeyframes = new Dictionary<int, ObservableCollection<KeyframeViewModel>>();

         foreach (var channelId in _lightSpecification.DmxChannels.Select(x => x.ChannelId))
            ChannelKeyframes.Add(channelId, new ObservableCollection<KeyframeViewModel>());

         BrightnessEffects = new ObservableCollection<EffectViewModelBase>();
         ColorEffects = new ObservableCollection<EffectViewModelBase>();
         DirectionEffects = new ObservableCollection<EffectViewModelBase>();

         SetKeyframes();
         SetEffects();
      }

      private void OnKeyframesChanged(object sender, EventArgs e)
      {
         SetKeyframes();
      }

      private void SetKeyframes()
      {
         BrightnessKeyframes.Clear();
         ColorKeyframes.Clear();
         DirectionKeyframes.Clear();

         foreach (var set in ChannelKeyframes)
            set.Value.Clear();

         _cue.GetBrightnessKeyframesFor(_light.Id).ToList().ForEach(f => BrightnessKeyframes.Add(_keyFrameViewModelFactory(f)));
         _cue.GetColorKeyframesFor(_light.Id).ToList().ForEach(f => ColorKeyframes.Add(_keyFrameViewModelFactory(f)));
         _cue.GetDirectionKeyframesFor(_light.Id).ToList().ForEach(f => DirectionKeyframes.Add(_keyFrameViewModelFactory(f)));

         foreach(var channelId in _lightSpecification.DmxChannels.Select(x => x.ChannelId))
         {
            _cue.GetChannelKeyframesFor(_light.Id, channelId).ToList().ForEach(f => ChannelKeyframes[channelId].Add(_keyFrameViewModelFactory(f)));
         }
      }

      private void SetEffects()
      {
         BrightnessEffects.Clear();
         ColorEffects.Clear();
         DirectionEffects.Clear();

         _cue.GetEffectsFor(_light.Id, LightProperty.Brightness).Select(CreateViewModel).ToList().ForEach(vm => BrightnessEffects.Add(vm));
         _cue.GetEffectsFor(_light.Id, LightProperty.Color).Select(CreateViewModel).ToList().ForEach(vm => ColorEffects.Add(vm));
         _cue.GetEffectsFor(_light.Id, LightProperty.Direction).Select(CreateViewModel).ToList().ForEach(vm => DirectionEffects.Add(vm));
      }

      private EffectViewModelBase CreateViewModel(EffectBase effect)
      {
         var type = effect.GetType();
         var viewModel = (EffectViewModelBase)_lifetimeScope.ResolveNamed(type.Name, typeof(EffectViewModelBase), new TypedParameter(effect.GetType(), effect));
         return viewModel;
      }

      public string Name { get { return _light.Name; } }

      public bool HasBrightnessKeyframes => _lightSpecification.HasBrightnessControl;

      public bool HasColorKeyframes => _lightSpecification.ColorControlType.CanChangeColor;

      public bool HasDirectionKeyframes => _lightSpecification.DirectionControlType.CanChangeDirection;

      public IEnumerable<string> ChannelLabels => _lightSpecification.DmxChannels.Select(x => $"{x.ChannelId}:{x.Name}");

      public Guid Id { get { return _light.Id; } }

      public ObservableCollection<KeyframeViewModel> BrightnessKeyframes { get; }

      public ObservableCollection<KeyframeViewModel> ColorKeyframes { get; }

      public ObservableCollection<KeyframeViewModel> DirectionKeyframes { get; }

      public ObservableCollection<EffectViewModelBase> BrightnessEffects { get; }

      public ObservableCollection<EffectViewModelBase> ColorEffects { get; }

      public ObservableCollection<EffectViewModelBase> DirectionEffects { get; }

      public Dictionary<int, ObservableCollection<KeyframeViewModel>> ChannelKeyframes { get; }

      internal void DeleteSelectedKeyframes()
      {
         var brightnessKeyframes = BrightnessKeyframes.Where(k => k.Selected).ToArray();
         foreach (var keyframe in brightnessKeyframes)
            _cue.RemoveBrightnessKeyframe(keyframe.Time);

         var colorKeyframes = ColorKeyframes.Where(k => k.Selected).ToArray();
         foreach (var keyframe in colorKeyframes)
            _cue.RemoveColorKeyframe(keyframe.Time);

         var directionKeyframes = DirectionKeyframes.Where(k => k.Selected).ToArray();
         foreach (var keyframe in directionKeyframes)
            _cue.RemoveDirectionKeyframe(keyframe.Time);

         var channelKeyframes = ChannelKeyframes.Values.SelectMany(x => x).Where(k => k.Selected).ToArray();
         foreach (var keyframe in channelKeyframes)
            _cue.RemoveChannelKeyframe(keyframe.Time, keyframe.ChannelId.Value);
      }
   }
}