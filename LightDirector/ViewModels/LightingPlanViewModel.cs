using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using LightDirector.Domain;
using LightDirector.Services;
using Microsoft.Win32;
using System.Windows.Media;
using LightDirector.Infrastructure.Services;
using LightDirector.Infrastructure.Domain;
using Prism.Events;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.ViewModels;
using Prism.Commands;

namespace LightDirector.ViewModels
{
   public class LightingPlanViewModel : ViewModelBase
   {
      private readonly LightingPlan _lightingPlan;
      private readonly TheatreModel _theatreModel;
      private CueViewModel _selectedCue;
      private readonly object _selectedCueLock = new object();
      private readonly object _lightsLock = new object();
      private readonly IAudioPlayer _audioPlayer;
      private readonly ICueTimeService _cueTimeService;
      private readonly IModeService _modeService;
      private IClientCueService _clientCueService;
      private IEventAggregator _eventAggregator;
      private readonly LightAddedEvent _lightAddedEvent;
      Func<ICue, LightingPlan, CueViewModel> _cueViewModelFactory;

      public LightingPlanViewModel(
         LightingPlan lightingPlan,
         IDmxService dmxService,
         IAudioPlayer audioPlayer,
         ICueTimeService cueTimeService,
         IClientCueService clientCueService,
         ILightStateService lightStateService,
         TheatreModel theatreModel,
         LightSpecificationRepository lightSpecificationRepository,
         IModeService modeService,
         IEventAggregator eventAggregator,
         ScreensModel screensModel,
         Func<ICue, LightingPlan, CueViewModel> cueViewModelFactory)
      {
         _screensModel = screensModel;
         _clientCueService = clientCueService;
         _cueTimeService = cueTimeService;
         _theatreModel = theatreModel;
         _modeService = modeService;
         _cueViewModelFactory = cueViewModelFactory;

         _eventAggregator = eventAggregator;

         _lightingPlan = lightingPlan;
         _audioPlayer = audioPlayer;

         OpenScene = new DelegateCommand(OpenSceneImpl);
         AddLight = new DelegateCommand(AddLightImpl);         
         NewReferenceCue = new DelegateCommand(NewReferenceCueImpl);

         PlayCommand = new DelegateCommand(cueTimeService.Start);
         PauseCommand = new DelegateCommand(cueTimeService.Stop);

         BirdsEyeViewCommand = new DelegateCommand(SetBirdsEyeView);
         AudienceViewCommand = new DelegateCommand(SetAudienceView);
         SetAudienceView();

         Run = new RunViewModel(_lightingPlan, dmxService);

         LightSpecifications = lightSpecificationRepository.RetrieveAll().OrderBy(s => s.Name);
         SelectedLightSpecification = LightSpecifications.First();

         if (!string.IsNullOrEmpty(lightingPlan.SceneFileName))
            SetScene(lightingPlan.SceneFileName);

         _theatreModel.SetAmbientLight(_lightingPlan.IsAmbientLightOn);

         var cueViewModels = CreateCueViewModels(lightingPlan.Cues);

         var referenceCueViewModels = CreateCueViewModels(lightingPlan.ReferenceCues);
         ReferenceCues = new ObservableCollection<CueViewModel>(referenceCueViewModels);

         _theatreModel.ModelChanged += OnSceneChanged;

         _lightAddedEvent = _eventAggregator.GetEvent<LightAddedEvent>();
         foreach (var light in lightingPlan.Lights)
            _lightAddedEvent.Publish(light);
      }

      public bool IsLiveMode
      {
         get { return _modeService.IsLive; }
         set
         {
            _modeService.SetLiveMode(value);
            _eventAggregator.GetEvent<LiveModeChangedEvent>().Publish(value);
            OnPropertyChanged();
         }
      }

      private Point3D _viewPosition;
      public Point3D ViewPosition
      {
         get { return _viewPosition; }
         set
         {
            _viewPosition = value;
            OnPropertyChanged();
         }
      }

      private Vector3D _viewDirection;
      private ScreensModel _screensModel;

      public Vector3D ViewDirection
      {
         get { return _viewDirection; }
         set
         {
            _viewDirection = value;
            OnPropertyChanged();
         }
      }

      public int Volume
      {
         get { return _audioPlayer.Volume; }
         set { _audioPlayer.Volume = value; }
      }

      private void SetBirdsEyeView()
      {
         ViewDirection = new Vector3D(-0.00005, -1, -0.0001);
         ViewPosition = new Point3D(0, 12, 1.5);
      }

      private void SetAudienceView()
      {
         ViewDirection = new Vector3D(-10, 0, -31.7290856692625);
         ViewPosition = new Point3D(4, 2, 9);
      }

      private IEnumerable<CueViewModel> CreateCueViewModels(IEnumerable<ICue> cues)
      {
         var cueViewModels = cues.Select(c =>
            _cueViewModelFactory(c, _lightingPlan));
         return cueViewModels;
      }

      public DelegateCommand PlayCommand { get; }
      public DelegateCommand PauseCommand { get; }

      public DelegateCommand AudienceViewCommand { get; }

      public DelegateCommand BirdsEyeViewCommand { get; }

      public IEnumerable<LightSpecification> LightSpecifications { get; }

      public LightSpecification SelectedLightSpecification { get; set; }

      private void NewReferenceCueImpl()
      {
         var cue = new Domain.Cue("New Cue");
         var insertIndex = SelectedCue == null ? ReferenceCues.Count : ReferenceCues.IndexOf(SelectedCue) + 1;
         _lightingPlan.AddReferenceCue(insertIndex, cue);
         var cueViewModel = _cueViewModelFactory(cue, _lightingPlan);
         ReferenceCues.Insert(insertIndex, cueViewModel);
      }

      public event EventHandler<SceneChangedEventArgs> SceneChanged;

      public string Name { get { return _lightingPlan.Name; } }

      public ObservableCollection<CueViewModel> ReferenceCues { get; private set; }

      public ICommand OpenScene { get; private set; }

      public ICommand AddLight { get; private set; }

      public ICommand RecordNewCue { get; private set; }

      public ICommand NewReferenceCue { get; private set; }

      public ICommand Blackout { get; private set; }

      public RunViewModel Run { get; private set; }

      public bool IsAmbientLightOn
      {
         get { return _lightingPlan.IsAmbientLightOn; }
         set
         {
            _lightingPlan.IsAmbientLightOn = value;
            _theatreModel.SetAmbientLight(value);
         }
      }

      public void ForceRefresh()
      {
         if (_theatreModel.HasScene)
            OnSceneChanged();
      }

      public CueViewModel SelectedCue
      {
         get
         {
            lock (_selectedCueLock)
            {
               return _selectedCue;
            }
         }
         set
         {
            lock (_selectedCueLock)
            {
               _selectedCue = value;
               if (_selectedCue != null)
               {
                  _eventAggregator.GetEvent<SelectedCueChangedEvent>().Publish(_selectedCue.Cue);
                  _cueTimeService.SetTime(TimeSpan.Zero);
               }
               else
               {
                  _eventAggregator.GetEvent<SelectedCueChangedEvent>().Publish(null);
               }
            }
            OnPropertyChanged();
         }
      }

      private void OpenSceneImpl()
      {
         var dialog = new OpenFileDialog { Filter = "Xaml|*.xaml" };
         var result = dialog.ShowDialog();
         if (result.HasValue && result.Value)
            SetScene(dialog.FileName);
      }

      private void SetScene(string fileName)
      {
         try
         {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
               var userControl = XamlReader.Load(fileStream) as UserControl;
               if (userControl == null || !(userControl.Content is ModelVisual3D))
                  throw new InvalidOperationException(
                     "Xaml wasn't in expected format. Xaml must contain a UserControl with a ModelVisual3D as the first element.");
               _theatreModel.SetScene((ModelVisual3D)userControl.Content, _screensModel);
               _lightingPlan.SceneFileName = fileName;
            }
         }
         catch (XamlParseException ex)
         {
            DisplayLoadError(ex);
         }
         catch (InvalidOperationException ex)
         {
            DisplayLoadError(ex);
         }
         catch (IOException ex)
         {
            DisplayLoadError(ex);
         }
      }

      private void AddLightImpl()
      {
         lock (_selectedCueLock)
            lock (_lightsLock)
            {
               var id = Guid.NewGuid();
               var light = new Domain.Light(id, SelectedLightSpecification.Id, SelectedLightSpecification.Name, Colors.PaleGoldenrod, new Point3D(0, 15, 0), new Vector3D(0, -1, 0));
               _lightAddedEvent.Publish(light);
               _lightingPlan.AddLight(light);
            }
      }

      private void OnSceneChanged()
      {
         var scene = _theatreModel.GetModel();
         var evt = SceneChanged;
         if (evt != null)
            evt(this, new SceneChangedEventArgs(scene));
      }

      private static void DisplayLoadError(Exception ex)
      {
         MessageBox.Show("Couldn't load file: " + ex.Message, "Error");
      }      
   }
}