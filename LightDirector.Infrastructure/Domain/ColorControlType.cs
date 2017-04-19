using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LightDirector.Domain
{
   public class ColorControlType
   {
      private ColorControlType(bool canSetColor, bool canChangeColor, bool canBlendBetweenColors, bool hasRgbCapability, IEnumerable<Color> availableColors)
      {
         CanSetColor = canSetColor;
         CanChangeColor = canChangeColor;
         CanBlendBetweenColors = canBlendBetweenColors;
         HasRgbCapability = hasRgbCapability;
         AvailableColors = availableColors;
      }

      public bool CanSetColor { get; }

      public bool CanChangeColor { get; }

      public bool CanBlendBetweenColors { get; }

      public bool HasRgbCapability { get; }

      public IEnumerable<Color> AvailableColors { get; }

      public static ColorControlType None()
      {
         return new ColorControlType(false, false, false, false, new Color[0]);
      }

      public static ColorControlType DynamicRgb()
      {
         return new ColorControlType(true, true, true, true, new Color[0]);
      }

      public static ColorControlType DynamicColorSet(IEnumerable<Color> colorSet)
      {
         return new ColorControlType(true, true, false, false, colorSet);
      }

      public static ColorControlType Static()
      {
         return new ColorControlType(true, false, false, false, new Color[0]);
      }
   }
}
