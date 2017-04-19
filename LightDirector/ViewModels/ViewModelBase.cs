using System.ComponentModel;
using System.Runtime.CompilerServices;
using LightDirector.Annotations;

namespace LightDirector.ViewModels
{
   public abstract class ViewModelBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         var handler = PropertyChanged;
         if (handler != null) 
            handler(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}