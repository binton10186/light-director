using LightDirector.Infrastructure.Domain;
using System;

namespace LightDirector.Infrastructure.Services
{
   public interface ICueTimeService
   {
      TimeSpan Time { get; }

      void Start();
      void Stop();
      void Reset();

      event EventHandler<CueTimeTickEventArgs> TimeChanged;
      event EventHandler<EventArgs> TimeStopped;
      event EventHandler<EventArgs> TimeStarted;
      event EventHandler<CueTimeTickEventArgs> TimeJumped; 

      void SetTime(TimeSpan newTime);
   }
}