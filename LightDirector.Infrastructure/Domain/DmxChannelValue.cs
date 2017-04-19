using System;

namespace LightDirector.Domain
{
   public class DmxChannelValue
   {
      public DmxChannelValue(Guid lightId, int channelId, int value)
      {
         LightId = lightId;
         ChannelId = channelId;
         Value = value;
      }

      public Guid LightId { get; }

      public int ChannelId { get; }

      public int Value { get; }
   }
}
