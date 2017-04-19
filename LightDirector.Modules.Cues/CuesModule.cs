using LightDirector.Infrastructure;
using LightDirector.Modules.Cues.Views;
using Prism.Modularity;
using Prism.Regions;

namespace LightDirector.Modules.Cues
{
   public class CuesModule : IModule
   {
      private readonly IRegionManager _regionManager;

      public CuesModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.Cues, typeof(CuesView));
      }
   }
}
