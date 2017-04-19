using Autofac;
using LightDirector.Modules.Cues.ViewModels;

namespace LightDirector.Modules.Cues
{
   public class CuesModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<CuesModule>();

         builder.RegisterType<CuesViewModel>();
         builder.RegisterType<AddCueDialogViewModel>();
      }
   }
}
