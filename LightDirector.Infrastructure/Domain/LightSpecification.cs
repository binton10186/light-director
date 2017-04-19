using LightDirector.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightDirector.Domain
{

   public class LightSpecification
   {
      public LightSpecification(Guid id, string name)
      {
         Id = id;
         Name = name;
         DmxChannels = Enumerable.Empty<DmxChannelSpecification>();
      }

      public bool HasBrightnessControl { get; set; }

      public int BrightnessChannel { get; set; }

      public int RedChannel { get; set; }

      public int GreenChannel { get; set; }

      public int BlueChannel { get; set; }

      public int PanChannel { get; set; }

      public int TiltChannel { get; set; }

      public decimal MinPanAngle { get; set; }

      public decimal MaxPanAngle { get; set; }

      public decimal MinTiltAngle { get; set; }

      public decimal MaxTiltAngle { get; set; }

      public ColorControlType ColorControlType { get; set; }

      public DirectionControlType DirectionControlType {get; set;}

      public IEnumerable<DmxChannelSpecification> DmxChannels { get; set; }

      public Guid Id { get; }

      public string Name { get; }

      public override string ToString()
      {
         return Name;
      }
   }
}
