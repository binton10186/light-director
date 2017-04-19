using Autofac;
using LightDirector.Modules.TimeLine.ViewModels;
using LightDirector.ViewModels;

namespace LightDirector.Modules.TimeLine
{
   public class TimeLineModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<TimeLineModule>();

         builder.RegisterType<TimeLineViewModel>();
         builder.RegisterType<PropertyGroupViewModel>();
      }
   }
}
