using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Services;
using Prism.Events;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Services
{
   public interface IKeyFrameService
   {
      void SetColor(Guid lightId, Color color);

      void SetBrightness(Guid lightId, int brightness);

      void SetDirection(Guid lightId, Vector3D direction);

      void SetChannel(Guid lightId, int channelId, int value);
   }

   public class KeyFrameService : IKeyFrameService
   {
      private readonly ICueTimeService _cueTimeService;
      private readonly IEventAggregator _eventAggregator;

      private LightingPlan _lightingPlan;
      private ICue _cue;

      public KeyFrameService(ICueTimeService cueTimeService, IEventAggregator eventAggregator)
      {
         _cueTimeService = cueTimeService;
         _eventAggregator = eventAggregator;
         _eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);
      }     

      public void SetColor(Guid lightId, Color color)
      {
         var cue = _cue;
         if (cue == null)
            return;

         var time = _cueTimeService.Time;
         var keyframe = new ColorKeyframe(time, color);
         cue.AddKeyFrame(lightId, keyframe);
      }

      public void SetBrightness(Guid lightId, int brightness)
      {
         var cue = _cue;
         if (cue == null)
            return;

         var time = _cueTimeService.Time;
         var keyframe = new BrightnessKeyframe(time, brightness);
         cue.AddKeyFrame(lightId, keyframe);
      }

      public void SetDirection(Guid lightId, Vector3D direction)
      {
         var cue = _cue;
         if (cue == null)
            return;

         var time = _cueTimeService.Time;
         var keyframe = new DirectionKeyframe(time, direction);
         cue.AddKeyFrame(lightId, keyframe);
      }

      public void SetChannel(Guid lightId, int channelId, int value)
      {
         var cue = _cue;
         if (cue == null)
            return;

         var time = _cueTimeService.Time;
         var keyframe = new ChannelKeyframe(time, value, channelId);
         cue.AddKeyFrame(lightId, keyframe);
      }

      private void OnSelectedCueChanged(ICue cue)
      {
         _cue = cue;
      }
   }
}
