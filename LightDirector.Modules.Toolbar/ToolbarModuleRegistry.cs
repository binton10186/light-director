using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.Modules.Toolbar
{
   public class ToolbarModuleRegistry : Module
   {
      protected override void Load(ContainerBuilder builder)
      {
         builder.RegisterType<ToolbarModule>();
      }
   }
}
