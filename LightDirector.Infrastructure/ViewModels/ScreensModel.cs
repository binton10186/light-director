using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.ViewModels;
using Prism.Events;
using System.Threading.Tasks;
using System;

namespace LightDirector.Infrastructure.ViewModels
{
   public class ScreensModel
   {
      private bool _isLiveMode;

      public ScreensModel(IEventAggregator eventAggregator, ScreenModel left, ScreenModel center, ScreenModel right)
      {
         Left = left;
         Center = center;
         Right = right;

         eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);
         eventAggregator.GetEvent<LiveModeChangedEvent>().Subscribe(OnLiveModeChanged);
      }

      private void OnLiveModeChanged(bool isLiveMode)
      {
         _isLiveMode = isLiveMode;

         if(_isLiveMode)
         {
            Left.Stop();
            Center.Stop();
            Right.Stop();
         }
      }

      public ScreenModel Left { get; }
      public ScreenModel Right { get; }
      public ScreenModel Center { get; }

      private void OnSelectedCueChanged(ICue cue)
      {
         Left.Stop();
         Center.Stop();
         Right.Stop();

         if (!_isLiveMode && cue != null)
         {
            Task.Factory.StartNew(() =>
            {
               if (!string.IsNullOrEmpty(cue.LeftVideoFileName))
                  Left.Play(cue.LeftVideoFileName);
            });

            Task.Factory.StartNew(() =>
            {
               if (!string.IsNullOrEmpty(cue.CenterVideoFileName))
                  Center.Play(cue.CenterVideoFileName);
            });

            Task.Factory.StartNew(() =>
            {
               if (!string.IsNullOrEmpty(cue.RightVideoFileName))
                  Right.Play(cue.RightVideoFileName);
            });
         }
      }
   }
}
