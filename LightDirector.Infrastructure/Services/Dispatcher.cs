using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LightDirector.Services
{
   public interface IDispatcher
   {
      void BeginInvoke(Action action);
   }

   public class Dispatcher : IDispatcher
   {
      public void BeginInvoke(Action action)
      {
         Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
      }
   }
}
