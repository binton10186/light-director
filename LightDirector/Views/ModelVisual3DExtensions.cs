using System.IO;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace LightDirector
{
   public static class ModelVisual3DExtensions
   {
      public static ModelVisual3D Clone(this ModelVisual3D model)
      {
         using (var ms = new MemoryStream())
         {
            XamlWriter.Save(model, ms);
            ms.Position = 0;
            return (ModelVisual3D)XamlReader.Load(ms);
         }
      }
   }
}