using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using LightDirector.ViewModels;

namespace LightDirector.Modules.TimeLine.Views
{
   /// <summary>
   /// Interaction logic for TimeLine.xaml
   /// </summary>
   public partial class TimeLine : UserControl
   {
      private const int TotalMilliseconds = 1000000;
      private const int CanvasWidth = 10000;

      private readonly Dictionary<TimeSpan, Rectangle> _keyframes = new Dictionary<TimeSpan, Rectangle>();
      private readonly Dictionary<long, TextBlock> _periods = new Dictionary<long, TextBlock>();

      public TimeLine()
      {
         InitializeComponent();
      }

      public static DependencyProperty KeyframesProperty = DependencyProperty.RegisterAttached(
         "Keyframes",
         typeof (IList),
         typeof (TimeLine),
         new PropertyMetadata(null, OnKeyframesChanged));

      public IList Keyframes
      {
         get { return (IList)GetValue(KeyframesProperty); }
         set { SetValue(KeyframesProperty, value);}
      }

      public static DependencyProperty PeriodsProperty = DependencyProperty.Register(
         "Periods",
         typeof(IList),
         typeof(TimeLine),
         new PropertyMetadata(null, OnPeriodsChanged));

      public IList Periods
      {
         get { return (IList)GetValue(PeriodsProperty); }
         set { SetValue(PeriodsProperty, value); }
      }

      public static DependencyProperty SelectedKeyframeProperty = DependencyProperty.Register(
         "SelectedKeyframe",
         typeof (object),
         typeof (TimeLine),
         new PropertyMetadata(null));

      public object SelectedKeyframe
      {
         get { return GetValue(SelectedKeyframeProperty); }
         set { SetValue(SelectedKeyframeProperty, value);}
      }

      private static void OnKeyframesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         var timeLine = (TimeLine) d;
         var newKeyFrames = (IList<KeyframeViewModel>)e.NewValue;
         foreach (var keyFrame in newKeyFrames)
            timeLine.AddKeyFrameDiamond(keyFrame);

         var keyFrameObservableCollection = e.NewValue as ObservableCollection<KeyframeViewModel>;
         if (keyFrameObservableCollection != null)
            keyFrameObservableCollection.CollectionChanged += timeLine.OnKeyframeCollectionChanged;
      }

      private static void OnPeriodsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         var timeLine = (TimeLine)d;
         var newPeriods = (IList<EffectViewModelBase>)e.NewValue;
         foreach (var period in newPeriods)
            timeLine.AddPeriod(period);

         var periodObservableCollection = e.NewValue as ObservableCollection<EffectViewModelBase>;
         if (periodObservableCollection != null)
            periodObservableCollection.CollectionChanged += timeLine.OnPeriodCollectionChanged;
      }

      private void OnKeyframeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.Action == NotifyCollectionChangedAction.Reset)
         {
            var allKeyframes = _keyframes.Keys.ToArray();
            foreach (var keyframe in allKeyframes)
               RemoveKeyFrameDiamond(keyframe);
         }
         else
         {
            if (e.OldItems != null)
            {
               foreach (var item in e.OldItems.OfType<KeyframeViewModel>())
                  RemoveKeyFrameDiamond(item.Time);
            }

            if (e.NewItems != null)
            {
               foreach (var item in e.NewItems.OfType<KeyframeViewModel>())
                  AddKeyFrameDiamond(item);
            }
         }
      }
   
      private void OnPeriodCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.Action == NotifyCollectionChangedAction.Reset)
         {
            var allPeriods = _periods.Keys.ToArray();
            foreach (var period in allPeriods)
               RemovePeriod(period);
         }
         else
         {
            if(e.OldItems != null)
            {
               foreach (var item in e.OldItems.OfType<EffectViewModelBase>())
                  RemovePeriod(item.StartTime);
            }

            if(e.NewItems != null)
            {
               foreach (var item in e.NewItems.OfType<EffectViewModelBase>())
                  AddPeriod(item);
            }
         }
      }

      private void RemoveKeyFrameDiamond(TimeSpan time)
      {
         Rectangle diamond;
         if (!_keyframes.TryGetValue(time, out diamond))
            return;
         
         _keyframes.Remove(time);
         TimeLineCanvas.Children.Remove(diamond);
      }

      private void RemovePeriod(long time)
      {
         TextBlock rectangle;
         if (!_periods.TryGetValue(time, out rectangle))
            return;

         _periods.Remove(time);
         TimeLineCanvas.Children.Remove(rectangle);
      }

      private void AddKeyFrameDiamond(KeyframeViewModel keyFrame)
      {
         var x = keyFrame.Time.TotalMilliseconds*CanvasWidth/TotalMilliseconds + 1;
         var transform = new RotateTransform(45d);
         var diamond = new Rectangle
         {
            Width = 5,
            Height = 5,
            Fill = new SolidColorBrush(Colors.Yellow),
            RenderTransform = transform,
            DataContext = keyFrame,
            Cursor = Cursors.Hand
         };
         diamond.SetValue(Canvas.LeftProperty, x);
         diamond.SetValue(Canvas.TopProperty, 16d);
         diamond.MouseLeftButtonDown += OnKeyframeClicked;
         

         var binding = new Binding
         {
            Source = diamond.DataContext,
            Path = new PropertyPath("Fill"),
            Mode = BindingMode.OneWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
         };
         BindingOperations.SetBinding(diamond, Rectangle.FillProperty, binding);
         
         TimeLineCanvas.Children.Add(diamond);
         _keyframes[keyFrame.Time] = diamond;
      }

      private void AddPeriod(EffectViewModelBase effect)
      {
         var startX = effect.StartTime * CanvasWidth / TotalMilliseconds + 1;
         var endX = effect.EndTime * CanvasWidth / TotalMilliseconds + 1;
         var width = endX - startX;
         var rectangle = new TextBlock
         {
            Width = width,
            Height = 23,
            Background = new SolidColorBrush(Colors.PaleGreen),
            DataContext = effect,
            Cursor = Cursors.Hand,
            Text = effect.Name
         };
         Canvas.SetLeft(rectangle, startX);
         rectangle.SetValue(Canvas.TopProperty, 6d);
         rectangle.MouseLeftButtonDown += OnPeriodClicked;

         TimeLineCanvas.Children.Add(rectangle);
         _periods[effect.StartTime] = rectangle;
      }

      private void OnPeriodClicked(object sender, MouseButtonEventArgs e)
      {
         if (e.ClickCount > 1)
         {
            var effectViewModel = (EffectViewModelBase)((TextBlock)sender).DataContext;
            effectViewModel.OpenView();
         }
      }

      private void OnKeyframeClicked(object sender, MouseButtonEventArgs e)
      {
         if(SelectedKeyframe != null)
            ((KeyframeViewModel)SelectedKeyframe).Selected = false;

         var keyframeViewModel = (KeyframeViewModel)((Rectangle) sender).DataContext;
         if (keyframeViewModel == null)
            return;

         SelectedKeyframe = keyframeViewModel;
         keyframeViewModel.Selected = true;

         if (e.ClickCount > 1)
            keyframeViewModel.SetTimeCommand.Execute();
      }
   }
}
