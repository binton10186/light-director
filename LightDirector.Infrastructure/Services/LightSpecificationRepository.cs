using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LightDirector.Services
{
   public class LightSpecificationRepository
   {
      private Dictionary<Guid, LightSpecification> _lightSpecifications = new Dictionary<Guid, LightSpecification>();

      public LightSpecificationRepository()
      {
         foreach (var specification in CreateSpecifications())
            _lightSpecifications.Add(specification.Id, specification);
      }

      public IEnumerable<LightSpecification> RetrieveAll()
      {
         return _lightSpecifications.Values.ToArray();
      }

      public LightSpecification RetrieveById(Guid specificationId)
      {
         return _lightSpecifications[specificationId];
      }

      private static IEnumerable<LightSpecification> CreateSpecifications()
      {
         yield return new LightSpecification(new Guid("ba422d0e-d3af-4c47-9a49-7965eed1d9ee"), "Generic Par Can")
         {
            HasBrightnessControl = true,
            BrightnessChannel = 1,
            ColorControlType = ColorControlType.Static(),
            DirectionControlType = DirectionControlType.Static()
         };

         yield return new LightSpecification(new Guid("2425d9ae-5cf3-4d1d-8fb7-c4c92b68fff6"), "Miltec Par Can 144 Q")
         {
            HasBrightnessControl = false,
            ColorControlType = ColorControlType.DynamicRgb(),
            RedChannel = 1,
            GreenChannel = 2,
            BlueChannel = 3,
            DirectionControlType = DirectionControlType.Static()
         };

         yield return new LightSpecification(new Guid("22f50966-86ee-4e50-b1a3-e1d229f21e5d"), "Martin Mac 250 Krypton")
         {
            HasBrightnessControl = true,
            BrightnessChannel = 2,
            ColorControlType = ColorControlType.Static(),
            DirectionControlType = DirectionControlType.Mover(),
            PanChannel = 13,
            TiltChannel = 15,
            MinTiltAngle = -128.5m,
            MaxTiltAngle = 128.5m,
            MinPanAngle = -270m,
            MaxPanAngle = 270m,
            DmxChannels = new[]
            {
               new DmxChannelSpecification(1, "Shutter, Strobe, Reset, Lamp On/Off"),
               new DmxChannelSpecification(4, "Color, Stepped Scroll, Continuous Rotation, Random Color"),
               new DmxChannelSpecification(6, "Gobo"),
               new DmxChannelSpecification(7, "Gobo Rotation, Continuous Rotation"),
               new DmxChannelSpecification(9, "Gobo wheel 2"),
               new DmxChannelSpecification(10, "Focus"),
               new DmxChannelSpecification(12, "Prism, Prism/Gobo Macros"),
               new DmxChannelSpecification(17, "Pan/Tilt Speed"),
               new DmxChannelSpecification(18, "Effects Speed, Color, Gobo Selection, Indexed Gobo Rotation, Prism")

            }
         };
      }      
   }
}
