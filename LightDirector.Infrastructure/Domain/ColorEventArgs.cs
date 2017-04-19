using System;
using System.Windows.Media;

namespace LightDirector.Infrastructure.Domain
{
   public class ColorEventArgs
   {
      public ColorEventArgs(Guid lightId, Color color)
      {
         LightId = lightId;
         Color = color;
      }

      public Guid LightId { get; private set; }
      public Color Color { get; private set; }
   }
}
