using System;
using System.IO;
using System.Windows.Media;
using LightDirector.Infrastructure.Services;
using Prism.Events;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Domain;
using System.Threading.Tasks;

namespace LightDirector.Services
{
   public class AudioPlayer : IAudioPlayer
   {
      private MediaPlayer _soundPlayer;
      private string _filename;
      private readonly object _lock = new object();
      private double _volume = 0.5d;
      private short _fileVolume;
      private readonly IDispatcher _dispatcher;
      private readonly IEventAggregator _eventAggregator;
      private ICue _currentCue;

      public AudioPlayer(
         ICueTimeService cueTimeService, 
         IDispatcher dispatcher,
         IEventAggregator eventAggregator)
      {
         _eventAggregator = eventAggregator;
         _dispatcher = dispatcher;

         _eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);

         cueTimeService.TimeStopped += (o, a) =>
         {
            _dispatcher.BeginInvoke(() =>
            {
               lock (_lock)
               {
                  if (_soundPlayer != null)
                     _soundPlayer.Pause();
               }
            });
         };

         cueTimeService.TimeStarted += (o, a) =>
         {
            _dispatcher.BeginInvoke(() =>
            {
               lock (_lock)
               {
                  if (!string.IsNullOrEmpty(_filename) && _soundPlayer != null)
                     _soundPlayer.Play();
               }
            });
         };

         cueTimeService.TimeJumped += (o, a) =>
         {
            _dispatcher.BeginInvoke(() =>
            {
               lock (_lock)
               {
                  if (!string.IsNullOrEmpty(_filename) && _soundPlayer != null)
                     _soundPlayer.Position = a.CueTimeElapsed;
               }
            });
         };
      }

      private void OnSelectedCueChanged(ICue cue)
      {
         var fadeout = 0d;
         if (_currentCue != null)
            fadeout = _currentCue.AudioFadeout;
         
         Stop(fadeout);
         _currentCue = cue;

         if (cue != null)
         {
            
            Task.Factory.StartNew(() =>
               Play(cue.AudioFileName, cue.Volume));
         }
      }

      private void Play(string fileName, short fileVolume)
      {
         _dispatcher.BeginInvoke(() =>
         {
            lock (_lock)
            {
               if (_soundPlayer != null)
               {
                  _soundPlayer.Stop();
                  _soundPlayer.Close();
               }

               _filename = fileName;
               _fileVolume = fileVolume;
               if (!File.Exists(fileName))
                  return;
               var volume = GetMediaVolume();

               _soundPlayer = new MediaPlayer { Volume = volume, ScrubbingEnabled = true };
               _soundPlayer.Open(new Uri(fileName));
               _soundPlayer.Play();
            }
         });
      }

      private double GetMediaVolume()
      {
         return _volume * _fileVolume / 10;
      }

      public void Stop(double fadeoutSeconds = 0)
      {
         lock (_lock)
         {
            if (_soundPlayer == null)
               return;

            var startVolume = _soundPlayer.Volume;
            var fadeStart = DateTime.Now;
            var fadeComplete = false;
            var fadeout = TimeSpan.FromSeconds(fadeoutSeconds);
            var fadeEnd = fadeStart + fadeout;
            while (!fadeComplete)
            {
               var timeNow = DateTime.Now;
               if (timeNow < fadeEnd)
               {
                  var fraction = 1 - ((timeNow - fadeStart).TotalSeconds / fadeoutSeconds);
                  _soundPlayer.Volume = startVolume * fraction;
               }
               else
               {
                  fadeComplete = true;
               }
            }

            _soundPlayer.Stop();
            _soundPlayer = null;
         }
      }

      public int Volume
      {
         get { return Convert.ToInt32(_volume * 100); }
         set
         {
            _dispatcher.BeginInvoke(() =>
            {
               lock (_lock)
               {
                  _volume = value / 100d;
                  var volume = GetMediaVolume();

                  if (_soundPlayer != null)
                     _soundPlayer.Volume = volume;
               }
            });
         }
      }
   }
}

