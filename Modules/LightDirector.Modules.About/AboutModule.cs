using LightDirector.Infrastructure;
using LightDirector.Modules.About.Views;
using Prism.Regions;

namespace LightDirector.Modules.About
{
   public class AboutModule : Prism.Modularity.IModule
   {
      private readonly IRegionManager _regionManager;

      public AboutModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.About, typeof(AboutView));
      }      
   }
}
