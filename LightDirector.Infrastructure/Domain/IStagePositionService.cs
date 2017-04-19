using System;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   public interface IStagePositionService
   {
      event Action<Point3D> CurrentPositionChanged;

      void SetPosition(Point3D position);

      Point3D CurrentPosition { get; }
   }
}