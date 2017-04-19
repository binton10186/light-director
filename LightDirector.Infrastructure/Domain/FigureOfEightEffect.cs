using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   public class FigureOfEightEffect : EffectBase
   {
      public override long EndMs { get; set; }

      public override long StartMs { get; set; }

      public double StartX { get; set; }

      public double StartZ { get; set; }

      public double EndX { get; set; }

      public double EndZ { get; set; }

      public double Amplitude { get; set; }

      public int PeriodMs { get; set; }

      public int OffsetMs { get; set; }

      public override int GetBrightness(TimeSpan time)
      {
         throw new NotImplementedException();
      }

      public override Color GetColor(TimeSpan time)
      {
         throw new NotImplementedException();
      }

      public override Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService)
      {
         var ms = Convert.ToInt64(time.TotalMilliseconds + OffsetMs) % PeriodMs;
         
         var startPoint = new Point3D(StartX, 0, StartZ);
         var endPoint = new Point3D(EndX, 0, EndZ);

         var length = EndX - StartX;

         var z = 0.0d;
         var x = 0.0d;
         if(ms < PeriodMs/2)
         {
            var factor = ((double)ms / PeriodMs) * 2;
            var angle = Math.PI * 2 * factor;
            z = Math.Sin(angle) * Amplitude;
            x = StartX + length * factor;
         }  
         else
         {
            var factor = (1 - ((double)ms / PeriodMs)) * 2;
            var angle = (2 * Math.PI) - (Math.PI * 2 * factor);
            z = Math.Sin(angle) * Amplitude;
            x = StartX + length * factor;
         }
         
         var point = new Point3D(x, 0, StartZ + z);
         Console.WriteLine($"{point.X},{point.Z}");
         var vector = point - light.Position;
         return vector;
      }

      public override bool IsFor(LightProperty property)
      {
         return property == LightProperty.Direction;
      }
   }
}
