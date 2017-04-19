using System;
using System.Windows.Media.Media3D;

namespace LightDirector.Infrastructure.Domain
{
   public class DirectionEventArgs
   {
      public DirectionEventArgs(Guid lightId, Vector3D direction)
      {
         LightId = lightId;
         Direction = direction;
      }

      public Guid LightId { get; }

      public Vector3D Direction { get; }
   }
}
