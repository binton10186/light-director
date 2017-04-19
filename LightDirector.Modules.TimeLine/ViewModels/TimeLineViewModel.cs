using LightDirector.Infrastructure.Events;
using LightDirector.ViewModels;
using Prism.Events;
using System.Collections.ObjectModel;
using LightDirector.Domain;
using System;
using LightDirector.Infrastructure.Domain;
using Prism.Commands;
using System.Windows.Input;
using LightDirector.Infrastructure.Services;
using Prism.Mvvm;

namespace LightDirector.Modules.TimeLine.ViewModels
{
   public class TimeLineViewModel : BindableBase
   {
      private readonly IEventAggregator _eventAggregator;
      private readonly Func<ICue, Light, PropertyGroupViewModel> _propertyGroupViewModelFactory;
      private readonly ICueTimeService _cueTimeService;

      private LightingPlan _lightingPlan;
      private ICue _cue;      

      public TimeLineViewModel(
         IEventAggregator eventAggregator,
         ICueTimeService cueTimeService,
         Func<ICue, Light, PropertyGroupViewModel> propertyGroupViewModelFactory)
      {
         _eventAggregator = eventAggregator;
         _propertyGroupViewModelFactory = propertyGroupViewModelFactory;
         _cueTimeService = cueTimeService;

         _cueTimeService.TimeChanged += OnTimeChanged;

         _eventAggregator.GetEvent<LightAddedEvent>().Subscribe(OnLightAdded);
         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(OnLightingPlanChanged);
         _eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);

         PropertyGroups = new ObservableCollection<PropertyGroupViewModel>();

         StopTime = new DelegateCommand(() => _cueTimeService.Stop());
         SetTime = new DelegateCommand<TimeSpan?>(t => _cueTimeService.SetTime(t.Value));
         DeleteKeyFrames = new DelegateCommand(DeleteKeyFramesImpl);
      }

      public ICommand StopTime { get; }

      public ICommand SetTime { get; }

      public ICommand DeleteKeyFrames { get; }

      public string Name => _cue?.Name;

      public TimeSpan Time
      {
         get
         {
            return _cueTimeService.Time;
         }
         set
         {
            if (value != null)
               _cueTimeService.SetTime(value);
         }
      }

      public ObservableCollection<PropertyGroupViewModel> PropertyGroups { get; }

      private void OnTimeChanged(object sender, CueTimeTickEventArgs e)
      {
         OnPropertyChanged(nameof(Time));
      }

      private void OnLightingPlanChanged(LightingPlan lightingPlan)
      {
         _lightingPlan = lightingPlan;
         RefreshPropertyGroups();
      }

      private void OnLightAdded(Light light)
      {
         RefreshPropertyGroups();
      }

      private void OnSelectedCueChanged(ICue cue)
      {
         _cue = cue;
         OnPropertyChanged(nameof(Name));
         RefreshPropertyGroups();
      }

      private void RefreshPropertyGroups()
      {
         if (_lightingPlan == null || _cue == null) return;

         PropertyGroups.Clear();
         foreach (var light in _lightingPlan.Lights)
            PropertyGroups.Add(_propertyGroupViewModelFactory(_cue, light));
      }

      private void DeleteKeyFramesImpl()
      {
         foreach (var propertyGroup in PropertyGroups)
            propertyGroup.DeleteSelectedKeyframes();
      }
   }
}
