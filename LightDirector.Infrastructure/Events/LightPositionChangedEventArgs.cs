using System;
using System.Windows.Media.Media3D;

namespace LightDirector.Infrastructure.Events
{
   public class LightPositionChangedEventArgs
   {
      public LightPositionChangedEventArgs(Guid lightId, Point3D position)
      {
         LightId = lightId;
         Position = position;
      }

      public Guid LightId { get; }

      public Point3D Position { get; }
   }
}
