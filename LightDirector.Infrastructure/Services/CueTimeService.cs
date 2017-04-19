using LightDirector.Infrastructure.Domain;
using System;
using System.Windows.Threading;

namespace LightDirector.Infrastructure.Services
{
   public class CueTimeService : ICueTimeService
   {
      const int RefreshIntervalMs = 16;

      private readonly DispatcherTimer _timer;
      private bool _isStarted = true;
      private DateTime _startTime;
      private TimeSpan _elapsedTime;

      public TimeSpan Time => _elapsedTime;

      public void Reset()
      {
         Stop();
         Start();
      }

      public event EventHandler<CueTimeTickEventArgs> TimeChanged;
      public event EventHandler<EventArgs> TimeStopped;
      public event EventHandler<EventArgs> TimeStarted;
      public event EventHandler<CueTimeTickEventArgs> TimeJumped; 

      public CueTimeService()
      {
         _timer = new DispatcherTimer
         {
            Interval = TimeSpan.FromMilliseconds(RefreshIntervalMs)
         };
         _timer.Tick += OnTimerTick;
         _startTime = DateTime.Now;
         _timer.Start();
      }

      public void Start()
      {
         _isStarted = true;
         _startTime = DateTime.Now - _elapsedTime;
         RaiseTimeStarted();
      }

      public void Stop()
      {
         _isStarted = false;
         RaiseTimeStopped();
      }

      public void SetTime(TimeSpan newTime)
      {
         _startTime = DateTime.Now - newTime;
         _elapsedTime = newTime;
         RaiseTimeChanged(newTime);
         RaiseTimeJumped(newTime);
      }

      private void OnTimerTick(object sender, EventArgs e)
      {
         if (_isStarted)
         {
            _elapsedTime = DateTime.Now - _startTime;
            RaiseTimeChanged(_elapsedTime);
         }
      }

      private void RaiseTimeStarted()
      {
         var evt = TimeStarted;
         if(evt != null)
            evt(this, new EventArgs());
      }

      private void RaiseTimeStopped()
      {
         var evt = TimeStopped;
         if(evt != null)
            evt(this, new EventArgs());
      }

      private void RaiseTimeChanged(TimeSpan elapsedTime)
      {
         var evt = TimeChanged;
         if (evt != null)
            evt(this, new CueTimeTickEventArgs(elapsedTime));
      }

      private void RaiseTimeJumped(TimeSpan elapsedTime)
      {
         var evt = TimeJumped;
         if (evt != null)
            evt(this, new CueTimeTickEventArgs(elapsedTime));
      }
   }
}