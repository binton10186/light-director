using System.Windows;
using LightDirector.Configuration;
using LightDirector.ViewModels;

namespace LightDirector
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         var autofacConfig = new AutofacConfig();
         autofacConfig.Run();     
      }      
   }
}
