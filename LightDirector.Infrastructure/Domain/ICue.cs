
using LightDirector.Domain;
using LightDirector.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace LightDirector.Infrastructure.Domain
{
   [Serializable]
   [XmlInclude(typeof(Cue))]
   [XmlInclude(typeof(ReferenceCue))]
   public abstract class ICue
   {
      public abstract event EventHandler KeyFramesChanged;

      [XmlIgnore]
      public abstract Guid Id { get; set; }

      [XmlIgnore]
      public abstract string Name { get; set; }

      [XmlIgnore]
      public abstract double FadeInSeconds { get; set; }

      [XmlIgnore]
      public abstract string AudioFileName { get; set; }

      [XmlIgnore]
      public abstract double AudioFadeout { get; set; }

      [XmlIgnore]
      public abstract Dictionary<Guid, List<EffectBase>> Effects { get; set; }

      [XmlIgnore]
      public abstract Dictionary<Guid, SortedDictionary<TimeSpan, BrightnessKeyframe>> BrightnessKeyframes { get; set; }

      [XmlIgnore]
      public abstract Dictionary<Guid, SortedDictionary<TimeSpan, ColorKeyframe>> ColorKeyframes { get; set; }

      [XmlIgnore]
      public abstract Dictionary<Guid, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>> ChannelKeyframes { get; set; }

      [XmlIgnore]
      public abstract string CenterVideoFileName { get; set; }

      [XmlIgnore]
      public abstract string LeftVideoFileName { get; set; }

      [XmlIgnore]
      public abstract string RightVideoFileName { get; set; }

      [XmlIgnore]
      public abstract short Volume { get; set; }

      public abstract IEnumerable<BrightnessKeyframe> GetBrightnessKeyframesFor(Guid lightId);

      public abstract IEnumerable<ColorKeyframe> GetColorKeyframesFor(Guid lightId);

      public abstract IEnumerable<DirectionKeyframe> GetDirectionKeyframesFor(Guid lightId);

      public abstract IEnumerable<ChannelKeyframe> GetChannelKeyframesFor(Guid lightId, int channelId);

      public abstract IEnumerable<EffectBase> GetEffectsFor(Guid lightId, LightProperty property);

      public abstract CueResult GetCueAt(TimeSpan time, IEnumerable<Light> lights, IStagePositionService stagePositionService, LightSpecificationRepository specificationRepository);

      public abstract void AddParCanSetting(ParCanSetting setting);

      public abstract void AddKeyFrame(Guid lightId, BrightnessKeyframe keyframe);
      public abstract void AddKeyFrame(Guid lightId, ColorKeyframe keyframe);

      public abstract void AddKeyFrame(Guid lightId, DirectionKeyframe keyframe);

      public abstract void AddKeyFrame(Guid lightId, ChannelKeyframe keyframe);

      public abstract void RemoveColorKeyframe(TimeSpan time);

      public abstract void RemoveBrightnessKeyframe(TimeSpan time);

      public abstract void RemoveDirectionKeyframe(TimeSpan time);

      public abstract void RemoveChannelKeyframe(TimeSpan time, int channelId);

      public ICue CreateFadeInCue(TimeSpan cueTime, TimeSpan fadeTime, ICue fadeToCue, IEnumerable<Light> lights, IStagePositionService stagePositionService, LightSpecificationRepository lightSpecificationRepository)
      {
         var cue = new Cue();
         

         foreach(var light in lights)
         {
            var specification = lightSpecificationRepository.RetrieveById(light.SpecificationId);

            cue.AddKeyFrame(light.Id, new BrightnessKeyframe(fadeTime, 0));
            cue.AddKeyFrame(light.Id, new ColorKeyframe(fadeTime, Colors.PaleGoldenrod));

            var brightnessKeyframes = fadeToCue.GetBrightnessKeyframesFor(light.Id);
            foreach(var keyframe in brightnessKeyframes.Where(k => k.Time > fadeTime))
               cue.AddKeyFrame(light.Id, new BrightnessKeyframe(keyframe.Time, keyframe.Brightness));

            var colorKeyframes = fadeToCue.GetColorKeyframesFor(light.Id);
            foreach (var keyframe in colorKeyframes.Where(k => k.Time > fadeTime))
               cue.AddKeyFrame(light.Id, new ColorKeyframe(keyframe.Time, keyframe.Color));

            var directionKeyframes = fadeToCue.GetDirectionKeyframesFor(light.Id);
            foreach (var keyframe in directionKeyframes.Where(k => k.Time > fadeTime))
               cue.AddKeyFrame(light.Id, new DirectionKeyframe(keyframe.Time, keyframe.Direction));

            foreach (var channelId in specification.DmxChannels.Select(c => c.ChannelId))
            {
               var channelKeyframes = fadeToCue.GetChannelKeyframesFor(light.Id, channelId);
               foreach (var keyframe in channelKeyframes.Where(k => k.Time > fadeTime))
                  cue.AddKeyFrame(light.Id, new ChannelKeyframe(keyframe.Time, keyframe.Value, keyframe.ChannelId));
            }
         }

         var cueResult = GetCueAt(cueTime, lights, stagePositionService, lightSpecificationRepository);
         foreach (var setting in cueResult.ParCanSettings)
         {
            cue.AddKeyFrame(setting.Id, new BrightnessKeyframe(TimeSpan.Zero, setting.Brightness));
            cue.AddKeyFrame(setting.Id, new ColorKeyframe(TimeSpan.Zero, setting.Color));
            cue.AddKeyFrame(setting.Id, new DirectionKeyframe(TimeSpan.Zero, setting.Direction));
            foreach (var channelValue in setting.ChannelValues)
               cue.AddKeyFrame(setting.Id, new ChannelKeyframe(TimeSpan.Zero, channelValue.Value, channelValue.ChannelId));
         }

         var fadeToCueResult = fadeToCue.GetCueAt(fadeTime, lights, stagePositionService, lightSpecificationRepository);
         foreach (var setting in fadeToCueResult.ParCanSettings)
         {
            cue.AddKeyFrame(setting.Id, new BrightnessKeyframe(fadeTime, setting.Brightness));
            cue.AddKeyFrame(setting.Id, new ColorKeyframe(fadeTime, setting.Color));
            cue.AddKeyFrame(setting.Id, new DirectionKeyframe(fadeTime, setting.Direction));
            foreach (var channelValue in setting.ChannelValues)
               cue.AddKeyFrame(setting.Id, new ChannelKeyframe(TimeSpan.Zero, channelValue.Value, channelValue.ChannelId));
         }

         foreach (var effect in fadeToCue.Effects)
         {
            cue.Effects.Add(effect.Key, effect.Value);
         }

         return cue;
      }
   }
}