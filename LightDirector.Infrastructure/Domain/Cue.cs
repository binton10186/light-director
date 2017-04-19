using LightDirector.Infrastructure.Domain;
using LightDirector.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   public class SerializedKeyframe
   {
      public Guid LightId { get; set; }

      public Keyframe Keyframe { get; set; }
   }

   [Serializable]
   public class Cue : ICue
   {
      public Cue()
      {
         //only for serialization
         BrightnessKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, BrightnessKeyframe>>();
         ColorKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, ColorKeyframe>>();
         DirectionKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, DirectionKeyframe>>();
         ChannelKeyframes = new Dictionary<Guid, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>>();
         Effects = new Dictionary<Guid, List<EffectBase>>();
         Volume = 10;
      }

      public Cue(string name)
      {
         Id = Guid.NewGuid();
         Name = name;
         BrightnessKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, BrightnessKeyframe>>();
         ColorKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, ColorKeyframe>>();
         DirectionKeyframes = new Dictionary<Guid, SortedDictionary<TimeSpan, DirectionKeyframe>>();
         ChannelKeyframes = new Dictionary<Guid, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>>();
         Effects = new Dictionary<Guid, List<EffectBase>>();
         Volume = 10;
      }

      [XmlAttribute("Id")]
      public override Guid Id { get; set; }

      public override string CenterVideoFileName { get; set; }

      public override string LeftVideoFileName { get; set; }

      public override string RightVideoFileName { get; set; }

      public override event EventHandler KeyFramesChanged;

      public override string Name { get; set; }

      public override short Volume { get; set; }

      public override double FadeInSeconds { get; set; }

      public override string AudioFileName { get; set; }

      public override double AudioFadeout { get; set; }

      public override CueResult GetCueAt(TimeSpan time, IEnumerable<Light> lights, IStagePositionService stagePositionService, LightSpecificationRepository lightSpecificationRepository)
      {
         var parCanSettings = new List<ParCanSetting>();
         var allLights = BrightnessKeyframes.Keys.Concat(ColorKeyframes.Keys).Concat(lights.Select(l => l.Id)).Distinct();
         foreach (var lightId in allLights)
         {
            var light = lights.SingleOrDefault(l => l.Id == lightId);
            var specification = lightSpecificationRepository.RetrieveById(light.SpecificationId);

            SortedDictionary<TimeSpan, BrightnessKeyframe> brightnesses;
            BrightnessKeyframes.TryGetValue(lightId, out brightnesses);

            SortedDictionary<TimeSpan, ColorKeyframe> colors;
            ColorKeyframes.TryGetValue(lightId, out colors);

            SortedDictionary<TimeSpan, DirectionKeyframe> directions;
            DirectionKeyframes.TryGetValue(lightId, out directions);

            SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe> channelKeyframes;
            ChannelKeyframes.TryGetValue(lightId, out channelKeyframes);

            List<EffectBase> effects;
            Effects.TryGetValue(lightId, out effects);

            brightnesses = brightnesses ?? new SortedDictionary<TimeSpan, BrightnessKeyframe>();
            colors = colors ?? new SortedDictionary<TimeSpan, ColorKeyframe>();
            directions = directions ?? new SortedDictionary<TimeSpan, DirectionKeyframe>();
            effects = effects ?? new List<EffectBase>();
            channelKeyframes = channelKeyframes ?? new SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>();

            var brightness = GetBrightness(time, brightnesses, specification);
            var color = GetColor(time, colors, light, effects);
            var direction = GetDirection(time, directions, light, effects, stagePositionService);
            var channelValues = GetChannelValues(time, channelKeyframes, light, lightSpecificationRepository);

            parCanSettings.Add(new ParCanSetting(lightId, Convert.ToInt32(brightness), color, direction, channelValues));
         }

         return new CueResult(parCanSettings.ToArray(), false);
      }

      private static Color GetColor(TimeSpan time, SortedDictionary<TimeSpan, ColorKeyframe> colors, Light light, List<EffectBase> effects)
      {
         var effect = effects.SingleOrDefault(e =>
            e.StartMs <= time.TotalMilliseconds &&
            e.EndMs > time.TotalMilliseconds &&
            e.IsFor(LightProperty.Color));

         if (effect != null)
         {
            return effect.GetColor(time);
         }
         else
         {
            if (!colors.Any())
               return light?.Color ?? Colors.PaleGoldenrod;

            var startFrame = colors.First().Value;

            var endFrame = startFrame;

            foreach (var keyframe in colors)
            {
               if (keyframe.Key < time)
               {
                  startFrame = keyframe.Value;
                  endFrame = keyframe.Value;
               }
               else if (keyframe.Key >= time)
               {
                  endFrame = keyframe.Value;
                  break;
               }
            }

            var timeFactor = endFrame.Time == startFrame.Time
               ? 0d
               : ((double)(time - startFrame.Time).Ticks) / ((double)(endFrame.Time - startFrame.Time).Ticks);

            var red = ((endFrame.Color.R - startFrame.Color.R) * timeFactor) + startFrame.Color.R;
            red = red < 0 ? 0 : red;
            red = red > 255 ? 255 : red;

            var green = ((endFrame.Color.G - startFrame.Color.G) * timeFactor) + startFrame.Color.G;
            green = green < 0 ? 0 : green;
            green = green > 255 ? 255 : green;

            var blue = ((endFrame.Color.B - startFrame.Color.B) * timeFactor) + startFrame.Color.B;
            blue = blue < 0 ? 0 : blue;
            blue = blue > 255 ? 255 : blue;

            return Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
         }
      }

      private static Vector3D GetDirection(TimeSpan time, SortedDictionary<TimeSpan, DirectionKeyframe> directions, Light light, List<EffectBase> effects, IStagePositionService stagePositionService)
      {
         var effect = effects.SingleOrDefault(e =>
            e.StartMs <= time.TotalMilliseconds &&
            e.EndMs > time.TotalMilliseconds &&
            e.IsFor(LightProperty.Direction));

         if (effect != null)
         {
            return effect.GetDirection(time, light, stagePositionService);
         }
         else
         {
            if (!directions.Any())
               return light?.Direction ?? new Vector3D(0, 1, 0);

            var startFrame = directions.First().Value;
            var endFrame = startFrame;
            foreach (var keyframe in directions)
            {
               if (keyframe.Key < time)
               {
                  startFrame = keyframe.Value;
                  endFrame = keyframe.Value;
               }
               else if (keyframe.Key >= time)
               {
                  endFrame = keyframe.Value;
                  break;
               }
            }

            var timeFactor = endFrame.Time == startFrame.Time
               ? 0d
               : ((double)(time - startFrame.Time).Ticks) / ((double)(endFrame.Time - startFrame.Time).Ticks);

            var x = ((endFrame.Direction.X - startFrame.Direction.X) * timeFactor) + startFrame.Direction.X;
            var y = ((endFrame.Direction.Y - startFrame.Direction.Y) * timeFactor) + startFrame.Direction.Y;
            var z = ((endFrame.Direction.Z - startFrame.Direction.Z) * timeFactor) + startFrame.Direction.Z;
            return new Vector3D(x, y, z);
         }
      }

      private static IEnumerable<DmxChannelValue> GetChannelValues(TimeSpan time, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe> channelKeyframes, Light light, LightSpecificationRepository lightSpecificationRepository)
      {
         var specification = lightSpecificationRepository.RetrieveById(light.SpecificationId);
         var channels = specification.DmxChannels;
         foreach(var channel in channels)
         {
            var value = 0;
            var specificChannelKeyframes = channelKeyframes.Where(k => k.Value.ChannelId == channel.ChannelId);
            if (specificChannelKeyframes.Any())
            {
               foreach(var keyframe in specificChannelKeyframes)
               {
                  if (keyframe.Key.Item2 <= time)
                  {
                     value = keyframe.Value.Value;
                  }
                  else
                  {
                     break;
                  }
               }
            }
            yield return new DmxChannelValue(light.Id, channel.ChannelId, value);
         }
      }

      private static double GetBrightness(TimeSpan time, SortedDictionary<TimeSpan, BrightnessKeyframe> brightnesses, LightSpecification specification)
      {
         if (!specification.HasBrightnessControl)
            return 100;

         if (!brightnesses.Any())
            return 0;

         var startFrame = brightnesses.First().Value;
         var endFrame = startFrame;

         foreach (var keyframe in brightnesses)
         {
            if (keyframe.Key < time)
            {
               startFrame = keyframe.Value;
               endFrame = keyframe.Value;
            }
            else if (keyframe.Key >= time)
            {
               endFrame = keyframe.Value;
               break;
            }
         }

         var timeFactor = endFrame.Time == startFrame.Time
            ? 0d
            : ((double)(time - startFrame.Time).Ticks) / ((double)(endFrame.Time - startFrame.Time).Ticks);

         var brightness = ((endFrame.Brightness - startFrame.Brightness) * timeFactor) + startFrame.Brightness;
         brightness = brightness < 0 ? 0 : brightness;
         brightness = brightness > 100 ? 100 : brightness;
         return brightness;
      }

      public override void AddParCanSetting(ParCanSetting setting)
      {
      }

      public override void AddKeyFrame(Guid lightId, BrightnessKeyframe keyframe)
      {
         SortedDictionary<TimeSpan, BrightnessKeyframe> lightKeyframes = null;
         if (!BrightnessKeyframes.TryGetValue(lightId, out lightKeyframes))
         {
            lightKeyframes = new SortedDictionary<TimeSpan, BrightnessKeyframe>();
            BrightnessKeyframes[lightId] = lightKeyframes;
         }
         lightKeyframes[keyframe.Time] = keyframe;


         RaiseKeyframesChanged();
      }

      public override void AddKeyFrame(Guid lightId, ColorKeyframe keyframe)
      {
         SortedDictionary<TimeSpan, ColorKeyframe> lightKeyframes = null;
         if (!ColorKeyframes.TryGetValue(lightId, out lightKeyframes))
         {
            lightKeyframes = new SortedDictionary<TimeSpan, ColorKeyframe>();
            ColorKeyframes[lightId] = lightKeyframes;
         }
         lightKeyframes[keyframe.Time] = keyframe;

         RaiseKeyframesChanged();
      }

      public override void AddKeyFrame(Guid lightId, DirectionKeyframe keyframe)
      {
         SortedDictionary<TimeSpan, DirectionKeyframe> directionKeyframes = null;
         if(!DirectionKeyframes.TryGetValue(lightId, out directionKeyframes))
         {
            directionKeyframes = new SortedDictionary<TimeSpan, DirectionKeyframe>();
            DirectionKeyframes[lightId] = directionKeyframes;
         }
         directionKeyframes[keyframe.Time] = keyframe;

         RaiseKeyframesChanged();
      }

      public override void AddKeyFrame(Guid lightId, ChannelKeyframe keyframe)
      {
         SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe> channelKeyframes = null;
         if(!ChannelKeyframes.TryGetValue(lightId, out channelKeyframes))
         {
            channelKeyframes = new SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>();
            ChannelKeyframes[lightId] = channelKeyframes;
         }
         channelKeyframes[new Tuple<int, TimeSpan>(keyframe.ChannelId, keyframe.Time)] = keyframe;

         RaiseKeyframesChanged();
      }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<TimeSpan, BrightnessKeyframe>> BrightnessKeyframes { get; set; }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<TimeSpan, ColorKeyframe>> ColorKeyframes { get; set; }

      [XmlIgnore]
      public Dictionary<Guid, SortedDictionary<TimeSpan, DirectionKeyframe>> DirectionKeyframes { get; set; }

      [XmlIgnore]
      public override Dictionary<Guid, SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>> ChannelKeyframes { get; set; }

      public SerializedKeyframe[] SerializedBrightnessKeyframes
      {
         get
         {
            return BrightnessKeyframes.SelectMany(k =>
               k.Value.Select(x =>
                  new SerializedKeyframe{LightId = k.Key, Keyframe = x.Value})).ToArray();
         }
         set
         {
            foreach (var serializedKeyframe in value)
            {
               var lightId = serializedKeyframe.LightId;
               var keyframe = (BrightnessKeyframe)serializedKeyframe.Keyframe;

               AddKeyFrame(lightId, keyframe);
            }
         }
      }

      public SerializedKeyframe[] SerializedColorKeyframes
      {
         get
         {
            return ColorKeyframes.SelectMany(k =>
               k.Value.Select(x =>
                  new SerializedKeyframe { LightId = k.Key, Keyframe = x.Value })).ToArray();
         }
         set
         {
            foreach (var serializedKeyframe in value)
            {
               var lightId = serializedKeyframe.LightId;
               var keyframe = (ColorKeyframe)serializedKeyframe.Keyframe;

               AddKeyFrame(lightId, keyframe);
            }
         }
      }

      public SerializedKeyframe[] SerializedDirectionKeyframes
      {
         get
         {
            return DirectionKeyframes.SelectMany(k =>
               k.Value.Select(x =>
                  new SerializedKeyframe { LightId = k.Key, Keyframe = x.Value })).ToArray();
         }
         set
         {
            foreach (var serializedKeyframe in value)
            {
               var lightId = serializedKeyframe.LightId;
               var keyframe = (DirectionKeyframe)serializedKeyframe.Keyframe;

               AddKeyFrame(lightId, keyframe);
            }
         }
      }

      public SerializedKeyframe[] SerializedChannelKeyframes
      {
         get
         {
            return ChannelKeyframes.SelectMany(k =>
               k.Value.Select(x => new SerializedKeyframe { LightId = k.Key, Keyframe = x.Value })).ToArray();
         }
         set
         {
            foreach(var serializedKeyframe in value)
            {
               var lightId = serializedKeyframe.LightId;
               var keyframe = (ChannelKeyframe)serializedKeyframe.Keyframe;

               AddKeyFrame(lightId, keyframe);
            }
         }
      }

      [XmlIgnore]
      public override Dictionary<Guid, List<EffectBase>> Effects { get; set; }

      public SerializedEffect[] SerializedEffects
      {
         get
         {
            return Effects.SelectMany(e => e.Value.Select(x => new SerializedEffect { LightId = e.Key, Effect = x })).ToArray();
         }
         set
         {
            foreach (var serializedEffect in value)
            {
               var lightId = serializedEffect.LightId;
               var effect = serializedEffect.Effect;

               AddEffect(lightId, effect);
            }
         }
      }

      public void AddEffect(Guid lightId, EffectBase effect)
      {
         List<EffectBase> effectsList;
         if(!Effects.TryGetValue(lightId, out effectsList))
         {
            effectsList = new List<EffectBase>();
            Effects[lightId] = effectsList;
         }

         effectsList.Add(effect);
      }

      public override IEnumerable<BrightnessKeyframe> GetBrightnessKeyframesFor(Guid lightId)
      {
         SortedDictionary<TimeSpan, BrightnessKeyframe> brightnessKeyframes;
         if (!BrightnessKeyframes.TryGetValue(lightId, out brightnessKeyframes))
            brightnessKeyframes = new SortedDictionary<TimeSpan, BrightnessKeyframe>();
         return brightnessKeyframes.Values.ToArray();
      }

      public override IEnumerable<ColorKeyframe> GetColorKeyframesFor(Guid lightId)
      {
         SortedDictionary<TimeSpan, ColorKeyframe> colorKeyframes;
         if (!ColorKeyframes.TryGetValue(lightId, out colorKeyframes))
            colorKeyframes = new SortedDictionary<TimeSpan, ColorKeyframe>();
         return colorKeyframes.Values.ToArray();
      }

      public override IEnumerable<DirectionKeyframe> GetDirectionKeyframesFor(Guid lightId)
      {
         SortedDictionary<TimeSpan, DirectionKeyframe> directionKeyframes;
         if (!DirectionKeyframes.TryGetValue(lightId, out directionKeyframes))
            directionKeyframes = new SortedDictionary<TimeSpan, DirectionKeyframe>();
         return directionKeyframes.Values.ToArray();
      }

      public override IEnumerable<ChannelKeyframe> GetChannelKeyframesFor(Guid lightId, int channelId)
      {
         SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe> channelKeyframes;
         if (!ChannelKeyframes.TryGetValue(lightId, out channelKeyframes))
            channelKeyframes = new SortedDictionary<Tuple<int, TimeSpan>, ChannelKeyframe>();
         return channelKeyframes.Values.Where(k => k.ChannelId == channelId).ToArray();
      }

      public override void RemoveColorKeyframe(TimeSpan time)
      {
         foreach(var lightKeyframes in ColorKeyframes.Values)
         {
            var keyframesToRemove = lightKeyframes.Where(kvp => kvp.Key == time).ToArray();
            foreach (var keyframeToRemove in keyframesToRemove)
               lightKeyframes.Remove(keyframeToRemove.Key);
         }        

         RaiseKeyframesChanged();
      }

      public override void RemoveBrightnessKeyframe(TimeSpan time)
      {
         foreach (var lightKeyframes in BrightnessKeyframes.Values)
         {
            var keyframesToRemove = lightKeyframes.Where(kvp => kvp.Key == time).ToArray();
            foreach (var keyframeToRemove in keyframesToRemove)
               lightKeyframes.Remove(keyframeToRemove.Key);
         }

         RaiseKeyframesChanged();
      }

      public override void RemoveDirectionKeyframe(TimeSpan time)
      {
         foreach(var directionKeyframes in DirectionKeyframes.Values)
         {
            var directionKeyframesToRemove = directionKeyframes.Where(kvp => kvp.Key == time).ToArray();
            foreach (var keyframeToRemove in directionKeyframesToRemove)
               directionKeyframes.Remove(keyframeToRemove.Key);
         }

         RaiseKeyframesChanged();
      }

      public override void RemoveChannelKeyframe(TimeSpan time, int channelId)
      {
         foreach (var channelKeyframes in ChannelKeyframes.Values)
         {
            var channelKeyframesToRemove = channelKeyframes.Where(kvp => kvp.Key.Item2 == time && kvp.Value.ChannelId == channelId).ToArray();
            foreach (var keyframeToRemove in channelKeyframesToRemove)
               channelKeyframes.Remove(keyframeToRemove.Key);
         }

         RaiseKeyframesChanged();
      }

      private void RaiseKeyframesChanged()
      {
         var evt = KeyFramesChanged;
         if (evt != null)
            evt(this, new EventArgs());
      }

      public override IEnumerable<EffectBase> GetEffectsFor(Guid lightId, LightProperty property)
      {
         List<EffectBase> effects;
         Effects.TryGetValue(lightId, out effects);

         return (effects ?? new List<EffectBase>()).Where(e => e.IsFor(property));
      }

      public override string ToString()
      {
         return Name;
      }
   }
}
