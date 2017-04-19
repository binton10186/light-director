using System;

namespace LightDirector.Infrastructure.Domain
{
   public class BrightnessEventArgs
   {
      public BrightnessEventArgs(Guid lightId, int brightness)
      {
         LightId = lightId;
         Brightness = brightness;
      }

      public Guid LightId { get; private set; }
      public int Brightness { get; private set; }
   }
}
