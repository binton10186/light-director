using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Reflection;

namespace LightDirector.Modules.About.ViewModels
{
   public class AboutViewModel : BindableBase, IInteractionRequestAware
   {
      public AboutViewModel()
      {
         CloseCommand = new DelegateCommand(Close);
      }      

      public Action FinishInteraction { get; set; }

      public INotification Notification { get; set; }

      public string ApplicationName
      {
         get
         {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return $"Light Director {assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}.{assemblyVersion.Revision}";
         }
      }

      public DelegateCommand CloseCommand { get; set; }

      private void Close()
      {
         FinishInteraction();
      }
   }
}
