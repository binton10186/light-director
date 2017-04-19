using System;
using LightDirector.Domain;
using System.Threading.Tasks;
using LightDirector.Services;
using Autofac;
using System.Windows;
using System.Windows.Media;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Services;
using Prism.Events;
using LightDirector.Infrastructure.Events;

namespace LightDirector.ViewModels
{

   public class CueViewModel : ViewModelBase
  {  
      private readonly ICue _cue;
      private readonly ICueTimeService _cueTimeService;
      private readonly IEventAggregator _eventAggregator;
      private readonly object _lightsLock = new object();
      private readonly ILightStateService _lightStateService;
      private readonly IKeyFrameService _keyframeService;
      private readonly Func<Keyframe, KeyframeViewModel> _keyFrameViewModelFactory;
      private readonly LightingPlan _lightingPlan;
      private readonly LightSpecificationRepository _lightSpecificationRepository;
      private readonly ILifetimeScope _lifetimeScope;
      private readonly IAudioPlayer _audioPlayer;
      private readonly IClientCueService _clientCueService;

      public CueViewModel(
         ICue cue, 
         LightingPlan lightingPlan,
         ICueTimeService cueTimeService,
         IClientCueService clientCueService,
         ILightStateService lightStateService,
         IKeyFrameService keyframeService,
         IEventAggregator eventAggregator,
         LightSpecificationRepository lightSpecificationRepository,
         ILifetimeScope lifetimeScope, 
         Func<Keyframe, KeyframeViewModel> keyFrameViewModelFactory,
         IAudioPlayer audioPlayer)
      {
         _lightStateService = lightStateService;
         _clientCueService = clientCueService;
         _cue = cue;
         _cueTimeService = cueTimeService;
         _lightingPlan = lightingPlan;
         _lightSpecificationRepository = lightSpecificationRepository;
         _lifetimeScope = lifetimeScope;
         _audioPlayer = audioPlayer;
         _eventAggregator = eventAggregator;

         _keyframeService = keyframeService;
         _keyFrameViewModelFactory = keyFrameViewModelFactory;     
      }

      public ICue Cue
      {
         get { return _cue; }
      }

      public Guid Id { get { return _cue.Id; } }

      public string Name
      {
         get { return _cue.Name; }
         set
         {
            _cue.Name = value;
            OnPropertyChanged();
         }
      }            

      public string AudioFileName
      {
         get { return _cue.AudioFileName; }
         set
         {
            _cue.AudioFileName = value;
            OnPropertyChanged();
         }
      }

      public string CenterVideoFileName
      {
         get { return _cue.CenterVideoFileName; }
         set { _cue.CenterVideoFileName = value; }
      }

      public string LeftVideoFileName
      {
         get { return _cue.LeftVideoFileName; }
         set { _cue.LeftVideoFileName = value; }
      }

      public string RightVideoFileName
      {
         get { return _cue.RightVideoFileName; }
         set { _cue.RightVideoFileName = value; }
      }

      public bool IsEditable
      {
         get { return true; }
      }
      
      public short Volume
      {
         get
         {
            return _cue.Volume;
         }
         set
         {
            _cue.Volume = value;
            OnPropertyChanged();
         }
      }

      public double AudioFadeout
      {
         get
         {
            return _cue.AudioFadeout;
         }
         set
         {
            _cue.AudioFadeout = value;
            OnPropertyChanged();
         }
      }

      public double FadeIn
      {
         get
         {
            return _cue.FadeInSeconds;
         }
         set
         {
            _cue.FadeInSeconds = value;
            OnPropertyChanged();
         }
      }

      public Visibility ReferenceCueVisibility => _cue is ReferenceCue ? Visibility.Visible : Visibility.Collapsed;

      public string ReferenceCue => (_cue as ReferenceCue)?.Underlying?.Name;

      public SolidColorBrush Background
      {
         get
         {
            var color = _cue is ReferenceCue ? Colors.PaleGreen : Colors.Transparent;
            return new SolidColorBrush(color);
         }
      }
   }
}
