using Autofac;
using LightDirector.Modules.About.ViewModels;
using LightDirector.Modules.About.Views;

namespace LightDirector.Modules.About
{
   public class AboutModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<AboutModule>();

         builder.RegisterType<AboutView>();
         builder.RegisterType<AboutViewModel>();
      }
   }
}
