using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LightDirector.Infrastructure.Domain;
using Prism.Commands;

namespace LightDirector.ViewModels
{
   public class RunViewModel : ViewModelBase
   {
      private readonly LightingPlan _lightingPlan;
      private readonly IDmxService _dmxService;

      public RunViewModel(LightingPlan lightingPlan, IDmxService dmxService)
      {
         _lightingPlan = lightingPlan;
         _dmxService = dmxService;

         StartDevice = new DelegateCommand(StartDeviceImpl);
         StopDevice = new DelegateCommand(StopDeviceImpl);

         if (lightingPlan != null)
         {
            var cues = _lightingPlan.Cues.Select(c => new RunCueViewModel(c)).ToArray();
            Cues = new ObservableCollection<RunCueViewModel>(cues);
         }
      }

      public ICommand StartDevice { get; private set; }
      public ICommand StopDevice { get; private set; }

      public ObservableCollection<RunCueViewModel> Cues { get; set; }

      public IEnumerable<RunLightViewModel> Lights
      {
         get
         {
            return _lightingPlan.Lights.Select(l => new RunLightViewModel(l)).ToArray();
         }
      }

      private RunCueViewModel _selectedCue;

      public RunCueViewModel SelectedCue
      {
         get
         {
            return _selectedCue;
         }
         set
         {
            _selectedCue = value;
         }

      }

      private void StartDeviceImpl()
      {
         _dmxService.Start();
      }

      private void StopDeviceImpl()
      {
         _dmxService.Stop();
      }
   }
}