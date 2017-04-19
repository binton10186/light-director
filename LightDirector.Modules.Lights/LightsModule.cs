using Autofac;
using LightDirector.Infrastructure;
using LightDirector.Modules.Lights.ViewModels;
using LightDirector.Modules.Lights.Views;
using Prism.Regions;
using System;

namespace LightDirector.Modules.Lights
{
   public class LightsModule : Module, Prism.Modularity.IModule
   {
      private readonly IRegionManager _regionManager;

      public LightsModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.Lights, typeof(LightsView));
      }      
   }
}
