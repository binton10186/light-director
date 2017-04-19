using System;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   public class ChannelKeyframe : Keyframe
   {
      public ChannelKeyframe()
      {
      }

      public ChannelKeyframe(TimeSpan time, int value, int channelId)
      {
         Value = value;
         Time = time;
         ChannelId = channelId;
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

      public int Value { get; set; }

      public long TimeTicks { get; set; }

      public int ChannelId { get; set; }
   }
}
