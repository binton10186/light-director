using LightDirector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LightDirector.Services
{
   public class StagePositionService : IStagePositionService
   {
      public event Action<Point3D> CurrentPositionChanged;

      public void SetPosition(Point3D position)
      {
         CurrentPosition = position;
         CurrentPositionChanged?.Invoke(position);
      }

      public Point3D CurrentPosition { get; private set; }
   }
}
