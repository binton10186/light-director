using LightDirector.Domain;
using System.Windows.Media.Media3D;

namespace LightDirector.ViewModels
{
   public class StageViewModel
   {
      private readonly IStagePositionService _positionService;

      public StageViewModel(IStagePositionService positionService)
      {
         _positionService = positionService;
      }

      public Point3D CurrentPosition
      {
         get
         {
            return new Point3D();
         }
         set
         {
            _positionService.SetPosition(value);
         }
      } 
   }
}
