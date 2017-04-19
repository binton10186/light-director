using System;
using System.Windows.Input;
using LightDirector.Infrastructure.Domain;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;

namespace LightDirector.Modules.Cues.ViewModels
{

   public class AddCueDialogViewModel : BindableBase, IInteractionRequestAware
   {
      private AddCueDialogNotification _notification;

      public AddCueDialogViewModel()
      {
         Add = new DelegateCommand(OnAddInvoked);
         Cancel = new DelegateCommand(OnCancelInvoked);
         ReferenceCues = new ObservableCollection<ICue>();
      }
      
      public string Name
      {
         get
         {
            return _notification?.Name;
         }

         set
         {
            _notification.Name = value;
         }
      }

      public ICommand Add { get; private set; }

      public ICommand Cancel { get; private set; }

      public ObservableCollection<ICue> ReferenceCues { get; }

      public ICue SelectedReferenceCue { get { return _notification?.ReferenceCue; } set { _notification.ReferenceCue = value; } }

      public INotification Notification
      {
         get
         {
            return _notification;
         }
         set
         {
            _notification = (AddCueDialogNotification)value;
            ReferenceCues.Clear();
            ReferenceCues.AddRange(_notification.ReferenceCues);
            OnPropertyChanged(nameof(ReferenceCues));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SelectedReferenceCue));
         }
      }

      public Action FinishInteraction { get; set; }

      private void OnAddInvoked()
      {
         _notification.Success = true;
         FinishInteraction();
      }

      private void OnCancelInvoked()
      {
         _notification.Success = false;
         FinishInteraction();
      }
   }
}