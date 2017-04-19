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
using System.Windows.Forms.Integration;
using CefSharp;
using CefSharp.WinForms;
using LightDirector.Modules.Viewer.ViewModels;
using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;

namespace LightDirector.Modules.Viewer.Views
{
   /// <summary>
   /// Interaction logic for ViewerView.xaml
   /// </summary>
   public partial class ViewerView : UserControl
   {
      private bool _isFirstLoad = true;
      private ChromiumWebBrowser _browser;

      public ViewerView()
      {
         Loaded += OnLoaded;

         InitializeComponent();
      }

      private void OnLightPositionChanged(LightPositionChangedEventArgs positionArgs)
      {
         var position = positionArgs.Position;
         var x = position.X * 100;
         var y = position.Y * 100;
         var z = position.Z * 100;
         var id = positionArgs.LightId;
         _browser.ExecuteScriptAsync($"updatePosition('{id}',{x},{y},{z});");
      }

      private void OnLightColorChanged(ColorEventArgs colorEventArgs)
      {
         var red = colorEventArgs.Color.R / 255.0;
         var green = colorEventArgs.Color.G / 255.0;
         var blue = colorEventArgs.Color.B / 255.0;
         _browser.ExecuteScriptAsync($"updateColor('{colorEventArgs.LightId}', {red}, {green}, {blue});");
      }

      private void OnLightBrightnessChanged(BrightnessEventArgs brightnessEventArgs)
      {
         _browser.ExecuteScriptAsync($"updateBrightness('{brightnessEventArgs.LightId}', {brightnessEventArgs.Brightness/100.0});");
      }

      private void OnLightAdded(Light light)
      {
         var position = light.Position;
         var x = position.X * 100;
         var y = position.Y * 100;
         var z = position.Z * 100;
         var id = light.Id;
         _browser.ExecuteScriptAsync($"addLight('{id}',{x},{y},{z});");
      }

      private void OnLoaded(object sender, RoutedEventArgs e)
      {
         if (_isFirstLoad)
         {
            Cef.Initialize();
            _browser = new ChromiumWebBrowser("file:///html/source.html");
            _browser.LoadingStateChanged += OnBrowserInitialized;
            var host = new WindowsFormsHost();
            host.Child = _browser;
            BrowserContainer.Children.Add(host);
            _isFirstLoad = false;
         }
      }

      private void OnBrowserInitialized(object sender, LoadingStateChangedEventArgs e)
      {
         if (!e.IsLoading)
         {
            Application.Current?.Dispatcher?.InvokeAsync(() =>
            {
               ((ViewerViewModel)DataContext).LightAdded += OnLightAdded;
               ((ViewerViewModel)DataContext).LightColorChanged += OnLightColorChanged;
               ((ViewerViewModel)DataContext).LightPositionChanged += OnLightPositionChanged;
               ((ViewerViewModel)DataContext).LightBrightnessChanged += OnLightBrightnessChanged;

               ((ViewerViewModel)DataContext).OnViewLoaded();
            });
         }
      }

      private void ToolsButton_Click(object sender, RoutedEventArgs e)
      {
         _browser.ShowDevTools();
      }

      private void RefreshButton_Click(object sender, RoutedEventArgs e)
      {
         _browser.Load("file:///html/source.html");
      }

      ~ViewerView()
      {
         Cef.Shutdown();
      }
   }
}
