using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   [Serializable]
   public class MovingSpotEffect : EffectBase
   {
      public override long EndMs { get; set; }

      public override long StartMs { get; set; }

      public override int GetBrightness(TimeSpan time)
      {
         throw new InvalidOperationException();
      }

      public override Color GetColor(TimeSpan time)
      {
         throw new InvalidOperationException();
      }

      public override Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService)
      {
         return stagePositionService.CurrentPosition - light.Position;
      }

      public override bool IsFor(LightProperty property)
      {
         return property == LightProperty.Direction;
      }
   }
}
