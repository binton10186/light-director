using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   [Serializable]
   public class ColorRepeatEffect : EffectBase
   {
      public List<Color> ColorSequence { get; set; }

      public int ColorDurationMs { get; set; }

      public override long StartMs { get; set; }

      public override long EndMs { get; set; }

      public override bool IsFor(LightProperty property)
      {
         return property == LightProperty.Color;
      }

      public override Color GetColor(TimeSpan time)
      {
         var effectTime = time.Subtract(TimeSpan.FromMilliseconds(StartMs));
         var singleSequenceTime = ColorSequence.Count * ColorDurationMs;
         var timeInCurrentSequence = effectTime.TotalMilliseconds % singleSequenceTime;
         var colorIndex = Convert.ToInt32(timeInCurrentSequence) / ColorDurationMs;

         colorIndex = colorIndex < 0 ? 0 : colorIndex;
         colorIndex = colorIndex >= ColorSequence.Count ? ColorSequence.Count - 1 : colorIndex;

         return ColorSequence.ElementAt(colorIndex);
      }

      public override Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService)
      {
         throw new InvalidOperationException();
      }

      public override int GetBrightness(TimeSpan time)
      {
         throw new InvalidOperationException();
      }
   }
}
