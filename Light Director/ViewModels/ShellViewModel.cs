using System;
using System.Windows;
using LightDirector.Services;
using LightDirector.ViewModels.Dialogs.NewProject;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Services;
using Prism.Events;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.ViewModels;

namespace LightDirector.ViewModels
{
   public class ShellViewModel : ViewModelBase, IDisposable
   {
      private LightingPlanViewModel _lightingPlanViewModel;
      private readonly ICueTimeService _cueTimeService;
      private IClientGateway _clientGateway;
      private readonly IModeService _modeService;
      private readonly IEventAggregator _eventAggregator;
      private Func<LightingPlan, ScreensModel, LightingPlanViewModel> _lightingPlanViewModelFactory;

      public ShellViewModel(
         ICueTimeService cueTimeService,
         IClientGateway clientGateway,
         IModeService modeService,
         IEventAggregator eventAggregator,
         Func<LightingPlan, ScreensModel, LightingPlanViewModel> lightingPlanViewModelFactory)
      {
         _lightingPlanViewModelFactory = lightingPlanViewModelFactory;
         _modeService = modeService;

         _eventAggregator = eventAggregator;
         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(OnLightingPlanChanged);

         _clientGateway = clientGateway;
         _clientGateway.Start();
         
         _cueTimeService = cueTimeService;        
      }      

      public LightingPlanViewModel LightingPlan
      {
         get
         {
            return _lightingPlanViewModel;
         }
         private set
         {
            _lightingPlanViewModel = value;
            OnPropertyChanged();
         }
      }

      public CueViewModel GetSelectedCue()
      {
         if (LightingPlan == null)
            return null;

         return LightingPlan.SelectedCue;
      }

      public void Dispose()
      {
         _clientGateway.Stop();
      }

      private void OnLightingPlanChanged(LightingPlan lightingPlan)
      {
         if (lightingPlan == null)
         {
            LightingPlan = null;
         }
         else
         {
            var centerScreen = new ScreenModel(new Point(-0.5, -2), new Point(2.17, -3.25), 2, _cueTimeService, _modeService);
            var leftScreen = new ScreenModel(new Point(-2.39, -0.11), new Point(-0.6, -2), 2, _cueTimeService, _modeService);
            var rightScreen = new ScreenModel(new Point(2.27, -3.25), new Point(4.58, -1.91), 2, _cueTimeService, _modeService);
            LightingPlan = _lightingPlanViewModelFactory(lightingPlan, new ScreensModel(_eventAggregator, leftScreen, centerScreen, rightScreen));
            LightingPlan.ForceRefresh();
         }
      }
   }
}
