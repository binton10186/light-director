using LightDirector.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.ViewModels.Dialogs.NewProject
{
   public class NewProjectDialogService
   {
      public NewProjectResult Show()
      {
         var vm = new NewProjectViewModel();
         var view = new NewProjectDialog { DataContext = vm };
         view.ShowDialog();
         var result = vm.GetResult();
         return result;
      }
   }
}
