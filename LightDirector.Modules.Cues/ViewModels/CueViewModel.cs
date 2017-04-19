using System;
using LightDirector.Domain;
using System.Windows;
using System.Windows.Media;
using LightDirector.Infrastructure.Domain;
using Prism.Mvvm;

namespace LightDirector.Modules.Cues.ViewModels
{

   public class CueViewModel : BindableBase
  {  
      private readonly ICue _cue;

      public CueViewModel(ICue cue)
      {
         _cue = cue;
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
