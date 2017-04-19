using LightDirector.Infrastructure.Domain;
using System.IO;
using System.Xml.Serialization;

namespace LightDirector.Modules.Menus.Services
{
   public class ProjectFileService
   {

      public LightingPlan Open(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            var serialiser = new XmlSerializer(typeof(LightingPlan));
            var lightingPlan = (LightingPlan)serialiser.Deserialize(fileStream);
            lightingPlan.Initialize();
            return lightingPlan;
         }
      }

      public void SaveAs(LightingPlan lightingPlan, string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
         {
            var serialiser = new XmlSerializer(typeof(LightingPlan));
            serialiser.Serialize(fileStream, lightingPlan);
         }
      }
   }
}
