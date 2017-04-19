using LightDirector.Infrastructure;
using LightDirector.Modules.Menus.Views;
using Prism.Modularity;
using Prism.Regions;

namespace LightDirector.Modules.Menus
{
   public class MenusModule : IModule
   {
      private readonly IRegionManager _regionManager;

      public MenusModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.Menu, typeof(MenuView));
      }
   }
}
