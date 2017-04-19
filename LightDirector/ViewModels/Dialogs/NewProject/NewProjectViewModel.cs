using Prism.Commands;
using System.Windows.Input;

namespace LightDirector.ViewModels.Dialogs.NewProject
{
   class NewProjectViewModel
   {
      public NewProjectViewModel()
      {
         Ok = new DelegateCommand(ExecuteOk);
      }

      private void ExecuteOk()
      {
         
      }

      public ICommand Ok { get; }

      public ICommand Cancel { get; }

      public NewProjectResult GetResult()
      {
         return new NewProjectResult();
      }
   }
}
