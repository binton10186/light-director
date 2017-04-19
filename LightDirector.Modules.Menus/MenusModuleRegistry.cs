using Autofac;
using LightDirector.Modules.Menus.Services;
using LightDirector.Modules.Menus.ViewModels;
using LightDirector.ViewModels;

namespace LightDirector.Modules.Menus
{
   public class MenusModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<MenusModule>();

         builder.RegisterType<MenuViewModel>();
         builder.RegisterType<StageViewModel>();

         builder.RegisterType<ProjectFileService>();
      }
   }
}
