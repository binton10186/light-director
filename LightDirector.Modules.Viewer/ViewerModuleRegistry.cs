using Autofac;
using LightDirector.Modules.Viewer.ViewModels;

namespace LightDirector.Modules.Viewer
{
   public class ViewerModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<ViewerModule>();

         builder.RegisterType<ViewerViewModel>();
      }
   }
}
