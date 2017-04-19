using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using LightDirector.ViewModels;
using System.Linq;

namespace LightDirector
{
   /// <summary>
   /// Interaction logic for LightingPlanView.xaml
   /// </summary>
   public partial class LightingPlanView : UserControl
   {
      private LightingPlanViewModel _viewModel;

      public LightingPlanView()
      {
         InitializeComponent();

         DataContextChanged += OnDataContextChanged;
      }

      private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         if (e.NewValue != null)
         {
            _viewModel = (LightingPlanViewModel)e.NewValue;
         }
      }

      private static void SetViewportToScene(ModelVisual3D scene, Viewport3D viewport)
      {
         viewport.Children.Clear();
         viewport.Children.Add(scene);
      }

      private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (e.AddedItems.Count > 0)
         {
            var listBoxItem = (ListBoxItem)ReferenceCueList
                  .ItemContainerGenerator
                  .ContainerFromItem(ReferenceCueList.SelectedItem);

            if (listBoxItem != null)
            {
               listBoxItem.Focus();
            }
         }
      }
   }
}
