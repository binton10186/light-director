using System;

namespace LightDirector.Infrastructure.Domain
{
   public class CueTimeTickEventArgs : EventArgs
   {
      public CueTimeTickEventArgs(TimeSpan cueTimeElapsed)
      {
         CueTimeElapsed = cueTimeElapsed;
      }

      public TimeSpan CueTimeElapsed { get; private set; }
   }
}