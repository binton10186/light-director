using Autofac;
using LightDirector.Infrastructure;
using LightDirector.Modules.Viewer.Views;
using Prism.Regions;

namespace LightDirector.Modules.Viewer
{
   public class ViewerModule : Module, Prism.Modularity.IModule
   {
      private readonly IRegionManager _regionManager;

      public ViewerModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.Viewer, typeof(ViewerView));
      }
   }
}
