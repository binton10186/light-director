using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   [Serializable]
   public class SwingingLightEffect : EffectBase
   {
      public override long EndMs { get; set; }

      public override long StartMs { get; set; }

      public override int GetBrightness(TimeSpan time)
      {
         throw new NotImplementedException();
      }

      public override Color GetColor(TimeSpan time)
      {
         return Colors.Black;
      }

      public double EndpointX { get; set; }

      public double EndpointZ { get; set; }

      public int SwingDurationMs { get; set; }

      public override Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService)
      {
         var ms = Convert.ToInt64(time.TotalMilliseconds) % Convert.ToInt64(SwingDurationMs);
         var endpoint = new Point3D(EndpointX, 0, EndpointZ);
         var endVector = endpoint - light.Position;
         var startVector = new Vector3D(0, -1, 0);

         var factor = 0.0d;
         if(ms < SwingDurationMs/2)
         {
            factor = (double)ms / SwingDurationMs;
         }
         else
         {
            factor = 1 - (double)ms / SwingDurationMs;
         }

         var partialVector = (endVector - startVector) * factor;
         var finalVector = startVector + partialVector;         
         return finalVector;
      }

      public override bool IsFor(LightProperty property)
      {
         return property == LightProperty.Direction;
      }
   }
}
