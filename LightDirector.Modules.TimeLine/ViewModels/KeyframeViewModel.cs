using System;
using System.Windows.Media;
using LightDirector.Domain;
using LightDirector.Infrastructure.Services;
using Prism.Mvvm;
using Prism.Commands;

namespace LightDirector.ViewModels
{
   public class KeyframeViewModel : BindableBase
   {
      private readonly ICueTimeService _cueTimeService;
      private readonly Keyframe _keyframe;

      public KeyframeViewModel(Keyframe keyframe, ICueTimeService cueTimeService)
      {
         Time = keyframe.Time;
         _cueTimeService = cueTimeService;
         SetTimeCommand = new DelegateCommand(() => _cueTimeService.SetTime(Time));
         _keyframe = keyframe;
      }

      public TimeSpan Time { get; private set; }

      public DelegateCommand SetTimeCommand { get; }

      private bool _selected;
      public bool Selected
      {
         get
         {
            return _selected;
         }
         set
         {
            _selected = value;
            OnPropertyChanged();
            OnPropertyChanged("Fill");
         }
      }

      public Brush Fill
      {
         get { return _selected ? new SolidColorBrush(Colors.DarkOrange) : new SolidColorBrush(Colors.Yellow); }
      }

      public int? ChannelId
      {
         get
         {
            var channelKeyframe = _keyframe as ChannelKeyframe;
            return channelKeyframe?.ChannelId;
         }
      }
   }
}