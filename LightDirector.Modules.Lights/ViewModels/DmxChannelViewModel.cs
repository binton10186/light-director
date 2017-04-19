using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Services;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace LightDirector.Modules.Lights.ViewModels
{
   public class DmxChannelViewModel : BindableBase
   {
      private readonly DmxChannelSpecification _dmxChannelSpecification;
      private readonly Guid _lightId;
      private readonly IKeyFrameService _keyframeService;

      public DmxChannelViewModel(
         Guid lightId, 
         DmxChannelSpecification dmxChannelSpecification,
         IKeyFrameService keyframeService,
         IEventAggregator eventAggregator)
      {
         _lightId = lightId;
         _dmxChannelSpecification = dmxChannelSpecification;

         eventAggregator.GetEvent<LightChannelValueChangedEvent>()
            .Subscribe(LightChannelValueChanged, ThreadOption.PublisherThread, false, 
             x => x.LightId == _lightId && x.ChannelId == ChannelId);

         _keyframeService = keyframeService;
      }

      private int _value;
      public int Value
      {
         get { return _value; }
         set
         {
            _keyframeService.SetChannel(_lightId, ChannelId, value);
         }
      }

      public string Name => _dmxChannelSpecification.Name;

      public int ChannelId => _dmxChannelSpecification.ChannelId;

      public string ChannelDisplayName => $"{ChannelId}: {Name}";

      private void LightChannelValueChanged(DmxChannelValue channelValue)
      {
         _value = channelValue.Value;
         OnPropertyChanged(nameof(Value));
      }
   }
}
