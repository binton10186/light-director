using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LightDirectoryClient
{
   public class MainWindowViewModel : INotifyPropertyChanged
   {
      private Uri _filename1;
      private Uri _filename2;

      private Visibility _visibility1;
      private Visibility _visibility2;

      private int _screen1ZIndex = 100;
      private int _screen2ZIndex = 1;

      private bool _usingElement1 = false;

      private string _lastFileName = null;
      private string _screen1FileName = "";
      private string _screen2FileName = "";

      private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

      public event PropertyChangedEventHandler PropertyChanged;

      public Uri Filename1
      {
         get
         {
            return _filename1;
         }
         set
         {
            _filename1 = value;
            OnPropertyChanged("Filename1");
         }
      }

      public Uri Filename2
      {
         get
         {
            return _filename2;
         }
         set
         {
            _filename2 = value;
            OnPropertyChanged("Filename2");
         }
      }

      public int Screen1ZIndex
      {
         get
         {
            return _screen1ZIndex;
         }
         set
         {
            _screen1ZIndex = value;
            OnPropertyChanged(nameof(Screen1ZIndex));
         }
      }

      public int Screen2ZIndex
      {
         get
         {
            return _screen2ZIndex;
         }
         set
         {
            _screen2ZIndex = value;
            OnPropertyChanged(nameof(Screen2ZIndex));
         }
      }

      public Visibility Visibility1
      {
         get
         {
            return _visibility1;
         }
         set
         {
            _visibility1 = value;
            OnPropertyChanged("Visibility1");
         }
      }

      public Visibility Visibility2
      {
         get
         {
            return _visibility2;
         }
         set
         {
            _visibility2 = value;
            OnPropertyChanged("Visibility2");
         }
      }

      private void OnPropertyChanged(string propertyName)
      {
         var propertyChanged = PropertyChanged;
         if (propertyChanged != null)
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }

      public void Show(string filename)
      {
         Application.Current.Dispatcher.BeginInvoke(new Action(() =>
         {
            Console.WriteLine("Dispatching");
            if (string.IsNullOrEmpty(filename))
            {
               Hide1();
               Hide2();
               _manualResetEvent.Set();
            }
            else if (filename != _lastFileName)
            {
               if (_usingElement1)
               {
                  Show2(filename);
                  _usingElement1 = false;

                  if (_screen2FileName == filename)
                  {
                     Hide1();
                     _manualResetEvent.Set();
                  }

                  _screen2FileName = filename;
               }
               else
               {
                  Show1(filename);
                  _usingElement1 = true;

                  if (_screen1FileName == filename)
                  {
                     Hide2();
                     _manualResetEvent.Set();
                  }

                  _screen1FileName = filename;
               }
            }
            else
            {
               _manualResetEvent.Set();
            }

            _lastFileName = filename;
         }), DispatcherPriority.Send);

         _manualResetEvent.WaitOne();
         _manualResetEvent.Reset();
      }

      public void Screen1Loaded()
      {         
         Hide2();
         _manualResetEvent.Set();
      }

      public void Screen2Loaded()
      {
         Hide1();
         _manualResetEvent.Set();
      }

      private void Hide1()
      {
         Console.WriteLine("Hide1");
         Visibility1 = Visibility.Collapsed;
      }

      private void Hide2()
      {
         Console.WriteLine("Hide2");
         Visibility2 = Visibility.Collapsed;
      }

      private void Show1(string filename)
      {
         Console.WriteLine("Show1");
         Screen1ZIndex = 1;
         Screen2ZIndex = 100;
         Filename1 = new Uri(filename);
         Visibility1 = Visibility.Visible;
      }

      private void Show2(string filename)
      {
         Console.WriteLine("Show2");
         Screen2ZIndex = 1;
         Screen1ZIndex = 100;
         Filename2 = new Uri(filename);
         Visibility2 = Visibility.Visible;
      }
   }
}
