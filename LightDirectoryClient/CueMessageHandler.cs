using System;
using System.Collections.Generic;
using System.Linq;
using LightDirectorMessages;
using System.Threading;

namespace LightDirectoryClient
{
   public class CueMessageHandler : IMessageHandler
   {
      private MainWindowViewModel _mainWindowViewModel;
      private readonly Thread _processingThread;
      private readonly Queue<Action> _processingQueue = new Queue<Action>();
      private bool _processing = true;

      public CueMessageHandler(MainWindowViewModel mainWindowViewModel)
      {
         _mainWindowViewModel = mainWindowViewModel;
         _processingThread = new Thread(ProcessQueue);
         _processingThread.Start();
      }
      
      public bool CanHandle(IMessage message)
      {
         return message is CueMessage;
      }

      public void Handle(IMessage message)
      {
         var cueMessage = (CueMessage)message;
         _processingQueue.Enqueue(() => _mainWindowViewModel.Show(cueMessage.Filename));
      }

      private void ProcessQueue()
      {
         while (_processing)
         {
            if (_processingQueue.Count > 0)
            {
               var action = _processingQueue.Dequeue();
               action();
            }
         }
      }

      public void Dispose()
      {
         _processing = false;
         _processingThread.Join();
      }
   }
}
