using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.ViewModels
{
   public class LightState
   {
      public LightState(Color color, int brightness, Vector3D direction)
      {
         Color = color;
         Brightness = brightness;
         Direction = direction;
      }

      public Color Color { get; }

      public int Brightness { get; }

      public Vector3D Direction { get; }
   }
}
