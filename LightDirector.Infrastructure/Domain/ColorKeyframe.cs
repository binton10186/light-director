using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   public class ColorKeyframe : Keyframe
   {
      public ColorKeyframe()
      {
      }

      public ColorKeyframe(TimeSpan time, Color color)
      {
         Time = time;
         Color = color;
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

      public Color Color { get; set; }

      public long TimeTicks { get; set; }
   }
}
