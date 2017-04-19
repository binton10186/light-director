using Autofac;
using LightDirector.Infrastructure;
using LightDirector.Modules.Toolbar.Views;
using Prism.Regions;

namespace LightDirector.Modules.Toolbar
{
   public class ToolbarModule : Module, Prism.Modularity.IModule
   {
      private readonly IRegionManager _regionManager;

      public ToolbarModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.Toolbar, typeof(ToolbarView));
      }
   }
}
