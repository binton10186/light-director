using System.Windows;
using System.Windows.Media;

namespace LightDirectoryClient
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      private LightDirectorGateway _gateway;
      private MainWindowViewModel _viewModel;

      public App()
      {
         RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
         _viewModel = new MainWindowViewModel();
         _gateway = new LightDirectorGateway(new IMessageHandler[] { new CueMessageHandler(_viewModel)});
      }

      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         _gateway.Connect();

         
         var window = new MainWindow();
         window.DataContext = _viewModel;
         window.Show();
         Application.Current.MainWindow = window;
      }

      protected override void OnExit(ExitEventArgs e)
      {
         base.OnExit(e);

         _gateway.Dispose();
      }
   }
}
