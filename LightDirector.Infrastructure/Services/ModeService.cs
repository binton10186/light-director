using System;

namespace LightDirector.Services
{
   public interface IModeService
   {
      event Action<bool> IsLiveChanged;

      void SetLiveMode(bool isLive);

      bool IsLive { get; }
   }

   public class ModeService : IModeService
   {
      private static readonly object LiveLock = new object();
      private bool _isLive = false;

      private event Action<bool> IsLiveChangedImpl;
      public event Action<bool> IsLiveChanged
      {
         add
         {
            lock(LiveLock)
            {
               IsLiveChangedImpl += value;
               value?.Invoke(_isLive);
            }
         }
         remove
         {
            lock(LiveLock)
            {
               IsLiveChangedImpl -= value;
            }
         }
      }

      public bool IsLive => _isLive;

      public void SetLiveMode(bool isLive)
      {
         lock(LiveLock)
         {
            _isLive = isLive;
            IsLiveChangedImpl?.Invoke(isLive);
         }
      }
   }
}
