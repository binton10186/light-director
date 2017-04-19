using System;
using Prism.Modularity;
using Prism.Regions;
using LightDirector.Infrastructure;
using LightDirector.Modules.TimeLine.Views;

namespace LightDirector.Modules.TimeLine
{
   public class TimeLineModule : IModule
   {
      private readonly IRegionManager _regionManager;

      public TimeLineModule(IRegionManager regionManager)
      {
         _regionManager = regionManager;
      }

      public void Initialize()
      {
         _regionManager.RegisterViewWithRegion(RegionNames.TimeLine, typeof(TimeLineView));
      }
   }
}
