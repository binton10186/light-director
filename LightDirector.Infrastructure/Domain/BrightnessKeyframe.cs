using System;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   public class BrightnessKeyframe : Keyframe
   {
      public BrightnessKeyframe()
      {
      }

      public BrightnessKeyframe(TimeSpan time, int brightness)
      {
         Time = time;
         Brightness = brightness;
      }

      [XmlIgnore]
      public override TimeSpan Time
      {
         get
         {
            return TimeSpan.FromTicks(TimeTicks);
         }
         set
         {
            TimeTicks = value.Ticks;
         } 
      }

      public int Brightness { get; set; }

      public long TimeTicks { get; set; }
   }
}