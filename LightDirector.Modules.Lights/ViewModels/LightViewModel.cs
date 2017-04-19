using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Services;
using LightDirector.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Modules.Lights.ViewModels
{
   public class LightViewModel : BindableBase
   {
      private readonly Domain.Light _light;
      private readonly ICueTimeService _cueTimeService;
      private readonly IKeyFrameService _keyframeService;
      private readonly LightSpecification _lightSpecification;
      private bool _stageSet;
      private readonly IEventAggregator _eventAggregator;

      private int _brightness;
      private Color _color;

      public LightViewModel(
         Domain.Light light,
         ICueTimeService cueTimeService,
         IKeyFrameService keyframeService,
         LightSpecificationRepository lightSpecificationRepository,
         IStagePositionService stagePositionService,
         IEventAggregator eventAggregator,
         Func<Guid, DmxChannelSpecification, DmxChannelViewModel> dmxChannelViewModelFactory)
      {
         _light = light;
         _cueTimeService = cueTimeService;
         _keyframeService = keyframeService;
         _lightSpecification = lightSpecificationRepository.RetrieveById(light.SpecificationId);
         _eventAggregator = eventAggregator;

         _brightness = 0;
         _color = Colors.PaleGoldenrod;
         SetDirectionFields(light.Direction);

         _eventAggregator.GetEvent<LightBrightnessChangedEvent>()
            .Subscribe(LightBrightnessChanged, ThreadOption.PublisherThread, false, e => e.LightId == _light.Id);
         _eventAggregator.GetEvent<LightColorChangedEvent>()
            .Subscribe(LightColorChanged, ThreadOption.PublisherThread, false, e => e.LightId == _light.Id);
         _eventAggregator.GetEvent<LightDirectionChangedEvent>()
            .Subscribe(LightDirectionChanged, ThreadOption.PublisherThread, false, e => e.LightId == _light.Id);

         ToggleStageSet = new DelegateCommand(() => { _stageSet = !_stageSet; });

         stagePositionService.CurrentPositionChanged += OnStagePositionChanged;
         DmxChannels = _lightSpecification.DmxChannels.OrderBy(c => c.ChannelId).Select(c => dmxChannelViewModelFactory(_light.Id, c));
      }

      private void OnStagePositionChanged(Point3D newPosition)
      {
         if (_stageSet)
            _keyframeService.SetDirection(_light.Id, newPosition - _light.Position);
      }

      private void SetDirectionFields(Vector3D direction)
      {
         var vector = CoordinateSystemConverter.ConvertToSphericalVector(direction);
         _incline = vector.Tilt;
         _rotation = vector.Pan;
      }

      public Domain.Light Light { get { return _light; } }

      public Guid Id
      {
         get { return _light.Id; }
      }

      public string Name
      {
         get { return _light.Name; }
         set
         {
            _light.Name = value;
            OnPropertyChanged();
         }
      }

      public double Height
      {
         get { return _light.Position.Y; }
         set
         {
            var position = new Point3D(_light.Position.X, value, _light.Position.Z);
            _light.Position = position;
            RaiseLightPositionChanged(position);
            OnPropertyChanged();
         }
      }

      public double X
      {
         get { return _light.Position.X; }
         set
         {
            var position = new Point3D(value, _light.Position.Y, _light.Position.Z);
            _light.Position = position;
            RaiseLightPositionChanged(position);
            OnPropertyChanged();
         }
      }

      public double Z
      {
         get { return _light.Position.Z; }
         set
         {
            var position = new Point3D(_light.Position.X, _light.Position.Y, value);
            _light.Position = position;
            RaiseLightPositionChanged(position);
            OnPropertyChanged();
         }
      }

      private int _rotation;
      public int Rotation
      {
         get { return _rotation; }
         set
         {
            _rotation = value;
            SetDirection();
            OnPropertyChanged();
         }
      }

      private int _incline = 0;
      public int Incline
      {
         get { return _incline; }
         set
         {
            _incline = value;
            SetDirection();
            OnPropertyChanged();
         }
      }

      public bool HasDirectionControl => _lightSpecification.DirectionControlType.CanSetDirection;

      private void SetDirection()
      {
         _cueTimeService.Stop();

         var phi = 180 - _incline;
         var theta = _rotation - 90;

         var x = Math.Sin(phi * Math.PI / 180) * Math.Cos(theta * Math.PI / 180);
         var y = Math.Cos(phi * Math.PI / 180);
         var z = Math.Sin(theta * Math.PI / 180) * Math.Sin(phi * Math.PI / 180);

         var direction = new Vector3D(x, y, z);
         _light.Direction = direction;

         if (_lightSpecification.DirectionControlType.CanChangeDirection)
         {
            _keyframeService.SetDirection(_light.Id, direction);
         }
         else
         {
            _light.Direction = direction;
         }
      }

      public bool HasBrightnessControl => _lightSpecification.HasBrightnessControl;

      public int Brightness
      {
         get { return _brightness; }
         set
         {
            _cueTimeService.Stop();
            SetBrightness(value);
            _keyframeService.SetBrightness(_light.Id, value);
         }
      }

      public bool HasColorControl => _lightSpecification.ColorControlType.CanSetColor;

      public Color Color
      {
         get { return _color; }

         set
         {
            _cueTimeService.Stop();
            SetColor(value);
            if (_lightSpecification.ColorControlType.CanChangeColor)
            {
               _keyframeService.SetColor(_light.Id, value);
            }
            else
            {
               _light.Color = value;
            }
         }
      }

      public int? Channel
      {
         get
         {
            var light = _light;
            if (light != null)
            {
               return light.ChannelId;
            }
            return null;
         }
         set
         {
            var light = _light;
            if (light != null && value != null)
            {
               light.ChannelId = value.Value;
            }

            OnPropertyChanged();
         }
      }

      public int MinimumPan => _lightSpecification.DirectionControlType.CanChangeDirection ? Convert.ToInt32(_lightSpecification.MinPanAngle) : 0;
      public int MaximumPan => _lightSpecification.DirectionControlType.CanChangeDirection ? Convert.ToInt32(_lightSpecification.MaxPanAngle) : 360;

      public int MinimumTilt => _lightSpecification.DirectionControlType.CanChangeDirection ? Convert.ToInt32(_lightSpecification.MinTiltAngle) : 0;

      public int MaximumTilt => _lightSpecification.DirectionControlType.CanChangeDirection ? Convert.ToInt32(_lightSpecification.MaxTiltAngle) : 180;

      public ICommand ToggleStageSet { get; }

      public IEnumerable<DmxChannelViewModel> DmxChannels { get; }

      private void LightBrightnessChanged(BrightnessEventArgs e)
      {
         SetBrightness(e.Brightness);
      }

      private void SetBrightness(int value)
      {
         if (_brightness != value)
         {
            _brightness = value;
            OnPropertyChanged(nameof(Brightness));
         }
      }

      private void LightColorChanged(ColorEventArgs e)
      {
         SetColor(e.Color);
      }

      private void SetColor(Color color)
      {
         if (_color != color)
         {
            _color = color;
            OnPropertyChanged(nameof(Color));
         }
      }

      private void RaiseLightPositionChanged(Point3D position)
      {
         _eventAggregator.GetEvent<LightPositionChangedEvent>().Publish(new LightPositionChangedEventArgs(Id, position));
      }

      private void LightDirectionChanged(DirectionEventArgs e)
      {
         SetDirectionFields(e.Direction);
         OnPropertyChanged(nameof(Incline));
         OnPropertyChanged(nameof(Rotation));         
      }
   }
}
