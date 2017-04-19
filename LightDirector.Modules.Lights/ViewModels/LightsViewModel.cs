using LightDirector.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LightDirector.Domain;

namespace LightDirector.Modules.Lights.ViewModels
{
   public class LightsViewModel : BindableBase
   {
      private readonly Func<Light, LightViewModel> _lightViewModelFactory;

      public LightsViewModel(
         IEventAggregator eventAggregator,
         Func<Light, LightViewModel> lightViewModelFactory)
      {
         _lightViewModelFactory = lightViewModelFactory;
         
         Lights = new ObservableCollection<LightViewModel>();
         Blackout = new DelegateCommand(BlackoutImpl);

         eventAggregator.GetEvent<LightAddedEvent>().Subscribe(OnLightAdded);
      }

      private void OnLightAdded(Light light)
      {
         Lights.Add(_lightViewModelFactory(light));
      }

      public ObservableCollection<LightViewModel> Lights { get; }
      
      public ICommand Blackout { get; }

      private void BlackoutImpl()
      {
         foreach (var light in Lights)
            light.Brightness = 0;
      }
   }
}
