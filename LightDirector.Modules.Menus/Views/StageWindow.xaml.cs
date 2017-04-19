using LightDirector.ViewModels;
using System;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace LightDirector
{
   /// <summary>
   /// Interaction logic for StageWindow.xaml
   /// </summary>
   public partial class StageWindow : Window
   {
      private StageViewModel _viewModel;

      public StageWindow()
      {
         InitializeComponent();

         DataContextChanged += StageWindow_DataContextChanged;
      }

      private void StageWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         _viewModel = (StageViewModel)e.NewValue;
      }

      private void Canvas_MouseMove(object sender, MouseEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            const int angle = 27;
            const int originX = 429;
            const int originY = 178;
            var position = e.GetPosition(StageCanvas);

            Canvas.SetLeft(PositionMarker, position.X);
            Canvas.SetTop(PositionMarker, position.Y);

            var x = ((position.X - originX) * Math.Sqrt(58) / 300) * Math.Cos(angle * Math.PI / 180) + ((position.Y - originY) * Math.Sqrt(41) / 500) * Math.Sin(angle * Math.PI / 180);
            var z = -((position.X - originX) * Math.Sqrt(58) / 300) * Math.Sin(angle * Math.PI / 180) + ((position.Y - originY) * Math.Sqrt(41) / 500) * Math.Cos(angle * Math.PI / 180);

            var point = new Point3D(x, 0, z);

            var vm = _viewModel;
            if (vm != null)
               vm.CurrentPosition = point;
         }
      }
   }
}
