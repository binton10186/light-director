using System;

namespace LightDirector.Domain
{
   [Serializable]
   public class SerializedEffect
   {
      public Guid LightId { get; set; }

      public EffectBase Effect { get; set; }
   }
}
