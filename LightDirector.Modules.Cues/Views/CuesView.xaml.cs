using LightDirector.Modules.Cues.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightDirector.Modules.Cues.Views
{
   /// <summary>
   /// Interaction logic for CuesView.xaml
   /// </summary>
   public partial class CuesView : UserControl
   {
      public CuesView()
      {
         InitializeComponent();
      }

      private void CueList_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Delete && CueList.SelectedIndex != -1)
         {
            ((CuesViewModel)DataContext).RemoveSelectedCue();
            CueList.SelectedIndex = -1;
         }
      }
   }
}
