using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightDirector.Modules.TimeLine.Views
{
   /// <summary>
   /// Interaction logic for TimeLineView.xaml
   /// </summary>
   public partial class StoryBoard : UserControl
   {
      public StoryBoard()
      {
         InitializeComponent();
      }

      private const long MaxMilliseconds = 1000000;

      private bool _movingTimeCursor;

      public string Header
      {
         get { return (string)GetValue(HeaderProperty); }
         set { SetValue(HeaderProperty, value); }
      }

      public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header",
        typeof(string),
        typeof(StoryBoard),
        new PropertyMetadata(null)
      );

      public TimeSpan Time
      {
         get { return (TimeSpan)GetValue(TimeProperty); }
         set { SetTimeValue(value); }
      }

      private void SetTimeValue(TimeSpan value)
      {
         SetValue(TimeProperty, value);
         SetTimeCursorPosition(value);
      }

      public static DependencyProperty TimeProperty = DependencyProperty.Register(
         "Time",
         typeof(TimeSpan),
         typeof(StoryBoard),
         new PropertyMetadata(default(TimeSpan), OnTimeChanged));

      public ICommand StopTime
      {
         get { return (ICommand)GetValue(StopTimeProperty); }
         set { SetValue(StopTimeProperty, value); }
      }

      public DependencyProperty StopTimeProperty = DependencyProperty.Register(
         "StopTime",
         typeof(ICommand),
         typeof(StoryBoard),
         new PropertyMetadata(null));

      public ICommand SetTime
      {
         get { return (ICommand)GetValue(SetTimeProperty); }
         set { SetValue(SetTimeProperty, value); }
      }

      public DependencyProperty SetTimeProperty = DependencyProperty.Register(
         "SetTime",
         typeof(ICommand),
         typeof(StoryBoard),
         new PropertyMetadata(null));

      public IList PropertyGroups
      {
         get { return (IList)GetValue(PropertyGroupsProperty); }
         set
         {
            SetValue(PropertyGroupsProperty, value);
         }
      }

      public static readonly DependencyProperty PropertyGroupsProperty = DependencyProperty.Register(
         "PropertyGroups",
         typeof(IList),
         typeof(StoryBoard),
         new PropertyMetadata(null));

      public static readonly DependencyProperty DeleteKeyframesCommandProperty = DependencyProperty.Register(
         "DeleteKeyframesCommand",
         typeof(ICommand),
         typeof(StoryBoard),
         new PropertyMetadata());

      public ICommand DeleteKeyframesCommand
      {
         get { return (ICommand)GetValue(DeleteKeyframesCommandProperty); }
         set { SetValue(DeleteKeyframesCommandProperty, value); }
      }

      private void StoryboardCanvas_MouseMove(object sender, MouseEventArgs e)
      {
         base.OnMouseMove(e);

         if (e.LeftButton != MouseButtonState.Pressed)
         {
            _movingTimeCursor = false;
         }

         if (_movingTimeCursor)
         {
            var relativePoint = e.GetPosition(StoryboardCanvas);
            var x = relativePoint.X;
            x = x < 0 ? 0 : x;
            x = x > StoryboardCanvas.ActualWidth ? StoryboardCanvas.ActualWidth : x;

            Canvas.SetLeft(TimeCursor, x);
            Canvas.SetTop(TimeCursor, 0);

            e.Handled = true;
         }
      }

      private void TimeCursor_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         _movingTimeCursor = true;
         TimeCursor.CaptureMouse();

         var stopTime = StopTime;
         if (stopTime != null)
            stopTime.Execute(null);
      }


      private void TimeCursor_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         _movingTimeCursor = false;
         TimeCursor.ReleaseMouseCapture();
      }

      private void TimeCursor_OnMouseMove(object sender, MouseEventArgs e)
      {
         if (!_movingTimeCursor)
            return;

         if (_movingTimeCursor)
         {
            var relativePoint = e.GetPosition(StoryboardCanvas);
            SetTimeFromXRelativeToCanvas(relativePoint.X);

            e.Handled = true;
         }
      }

      private void TimeBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         var position = e.GetPosition(StoryboardCanvas);
         SetTimeFromXRelativeToCanvas(position.X);
      }

      private void SetTimeFromXRelativeToCanvas(double x)
      {
         x = x < 0 ? 0 : x;
         x = x > StoryboardCanvas.ActualWidth ? StoryboardCanvas.ActualWidth : x;

         var timeInMs = MaxMilliseconds / StoryboardCanvas.ActualWidth * x;
         var time = TimeSpan.FromMilliseconds(timeInMs);

         if(SetTime != null)
            SetTime.Execute(time);
      }

      private void SetTimeCursorPosition(TimeSpan time)
      {
         var x = time.TotalMilliseconds * StoryboardCanvas.ActualWidth / MaxMilliseconds;
         x = x < 0 ? 0 : x;
         x = x > StoryboardCanvas.ActualWidth ? StoryboardCanvas.ActualWidth : x;

         Canvas.SetLeft(TimeCursor, x);
         Canvas.SetTop(TimeCursor, 0);
      }

      private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         var storyboard = (StoryBoard)d;
         storyboard.Time = (TimeSpan)e.NewValue;
      }

      protected override void OnKeyUp(KeyEventArgs e)
      {
         base.OnKeyUp(e);

         if (e.Key == Key.Delete)
         {
            var deleteCommand = DeleteKeyframesCommand;
            if (deleteCommand != null)
               deleteCommand.Execute(null);
         }
      }
   }
}
