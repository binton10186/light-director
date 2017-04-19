using System;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   public class DirectionKeyframe : Keyframe
   {
      public DirectionKeyframe()
      {
      }

      public DirectionKeyframe(TimeSpan time, Vector3D direction)
      {
         Time = time;
         Direction = direction;
      }

      public override TimeSpan Time { get; set; }

      public Vector3D Direction { get; set; }
   }
}
