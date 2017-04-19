using Autofac;
using LightDirector.Modules.Lights.ViewModels;

namespace LightDirector.Modules.Lights
{
   public class LightsModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<LightsModule>();

         builder.RegisterType<LightsViewModel>();
         builder.RegisterType<LightViewModel>();
         builder.RegisterType<DmxChannelViewModel>();
      }
   }
}
