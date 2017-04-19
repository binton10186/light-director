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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightDirectoryClient
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private bool _isMaximised = false;

      public MainWindow()
      {         
         InitializeComponent();
      }      

      protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
      {
         base.OnMouseDoubleClick(e);
         e.Handled = true;

         if (_isMaximised)
         {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;

            _isMaximised = false;
         }
         else
         {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            Hide();
            Show();
            BringIntoView();

            _isMaximised = true;
         }

      }

      private void Screen1_Loaded(object sender, RoutedEventArgs e)
      {
         if (DataContext == null)
            return;

         var dataContext = (MainWindowViewModel)DataContext;
         dataContext.Screen1Loaded();
      }

      private void Screen2_Loaded(object sender, RoutedEventArgs e)
      {
         if (DataContext == null)
            return;

         var dataContext = (MainWindowViewModel)DataContext;
         dataContext.Screen2Loaded();
      }
   }
}
