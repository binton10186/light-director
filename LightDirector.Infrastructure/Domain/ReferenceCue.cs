using LightDirector.Infrastructure.Domain;
using LightDirector.Services;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   public class ReferenceCue : ICue
   {
      public ReferenceCue()
      {            
      }

      public ReferenceCue(string name, ICue underlying)
      {
         Name = name;
         Underlying = underlying;
         UnderlyingId = Underlying.Id;
         Id = Guid.NewGuid();
      }

      [XmlIgnore]
      public ICue Underlying { get; set; }

      public Guid UnderlyingId { get; set; }
      
      public override string AudioFileName { get; set; }

      public override double AudioFadeout { get; set; }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<TimeSpan, BrightnessKeyframe>> BrightnessKeyframes
      {
         get { return Underlying.BrightnessKeyframes; }
         set { Underlying.BrightnessKeyframes = value; }
      }
      
      public override string CenterVideoFileName { get; set; }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<TimeSpan, ColorKeyframe>> ColorKeyframes
      {
         get { return Underlying.ColorKeyframes; }
         set { Underlying.ColorKeyframes = value; }
      }

      [XmlIgnore]
      public override Dictionary<Guid, List<EffectBase>> Effects
      {
         get { return Underlying.Effects; }
         set { Underlying.Effects = value; }
      }

      public override double FadeInSeconds { get; set; }
      
      public override Guid Id { get; set; }
      
      public override string LeftVideoFileName { get; set; }
      
      public override string Name { get; set; }
      
      public override string RightVideoFileName { get; set; }

      public override short Volume { get; set; }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>> ChannelKeyframes
      {
         get { return Underlying.ChannelKeyframes; }
         set { Underlying.ChannelKeyframes = value; }
      }

      public override event EventHandler KeyFramesChanged;

      public override void AddKeyFrame(Guid lightId, DirectionKeyframe keyframe)
      {
         Underlying.AddKeyFrame(lightId, keyframe);
      }

      public override void AddKeyFrame(Guid lightId, ColorKeyframe keyframe)
      {
         Underlying.AddKeyFrame(lightId, keyframe);
      }

      public override void AddKeyFrame(Guid lightId, BrightnessKeyframe keyframe)
      {
         Underlying.AddKeyFrame(lightId, keyframe);
      }

      public override void AddKeyFrame(Guid lightId, ChannelKeyframe keyframe)
      {
         Underlying.AddKeyFrame(lightId, keyframe);
      }

      public override void AddParCanSetting(ParCanSetting setting)
      {
         Underlying.AddParCanSetting(setting);
      }

      public override IEnumerable<BrightnessKeyframe> GetBrightnessKeyframesFor(Guid lightId)
      {
         return Underlying.GetBrightnessKeyframesFor(lightId);
      }

      public override IEnumerable<ColorKeyframe> GetColorKeyframesFor(Guid lightId)
      {
         return Underlying.GetColorKeyframesFor(lightId);
      }

      public override CueResult GetCueAt(TimeSpan time, IEnumerable<Light> lights, IStagePositionService stagePositionService, LightSpecificationRepository lightSpecificationRepository)
      {
         return Underlying.GetCueAt(time, lights, stagePositionService, lightSpecificationRepository);
      }

      public override IEnumerable<DirectionKeyframe> GetDirectionKeyframesFor(Guid lightId)
      {
         return Underlying.GetDirectionKeyframesFor(lightId);
      }

      public override IEnumerable<EffectBase> GetEffectsFor(Guid lightId, LightProperty property)
      {
         return Underlying.GetEffectsFor(lightId, property);
      }

      public override void RemoveBrightnessKeyframe(TimeSpan time)
      {
         Underlying.RemoveBrightnessKeyframe(time);
      }

      public override void RemoveColorKeyframe(TimeSpan time)
      {
         Underlying.RemoveColorKeyframe(time);
      }

      public override void RemoveDirectionKeyframe(TimeSpan time)
      {
         Underlying.RemoveDirectionKeyframe(time);
      }

      public override IEnumerable<ChannelKeyframe> GetChannelKeyframesFor(Guid lightId, int channelId)
      {
         return Underlying.GetChannelKeyframesFor(lightId, channelId);
      }

      public override void RemoveChannelKeyframe(TimeSpan time, int channelId)
      {
         Underlying.RemoveChannelKeyframe(time, channelId);
      }
   }
}
