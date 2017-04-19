using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LightDirector.Services
{
   public class SphericalVector
   {
      public SphericalVector(int tilt, int pan)
      {
         Tilt = tilt;
         Pan = pan;
      }

      public int Tilt { get; }
      public int Pan { get; }
   }

   public static class CoordinateSystemConverter
   {
      public static SphericalVector ConvertToSphericalVector(Vector3D cartesianVector)
      {
         var tilt = 0;

         var magnitude = Math.Sqrt(Math.Pow(cartesianVector.X, 2) + Math.Pow(cartesianVector.Y, 2) + Math.Pow(cartesianVector.Z, 2));
         var unitVector = new Vector3D(cartesianVector.X / magnitude, cartesianVector.Y / magnitude, cartesianVector.Z / magnitude);

         try
         {
            tilt = 180 - Convert.ToInt32(Math.Acos(unitVector.Y) * 180 / Math.PI);
         }
         catch (OverflowException)
         {
         }

         var pan = 0;
         try
         {
            if (cartesianVector.X != 0)
            { 
               var angleAddition = cartesianVector.X >= 0 ? 90 : -90;
               pan = angleAddition + Convert.ToInt32(Math.Atan(cartesianVector.Z / cartesianVector.X) * 180 / Math.PI);
            }
         }
         catch (OverflowException)
         {
            pan = 0;
         }

         return new SphericalVector(tilt, pan);
      }
   }
}
