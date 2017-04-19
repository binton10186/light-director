using Autofac;
using LightDirector.Domain;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Commands;

namespace LightDirector.ViewModels
{
   public class ColorRepeatEffectViewModel : EffectViewModelBase
   {
      private readonly ILifetimeScope _lifetimeScope;
      private ColorRepeatEffect _colorRepeatEffect;

      public ColorRepeatEffectViewModel(ColorRepeatEffect effect, ILifetimeScope lifetimeScope) : base(effect)
      {
         _lifetimeScope = lifetimeScope;
         _colorRepeatEffect = effect;

         AddColor = new DelegateCommand(AddColorImpl);
         RemoveColor = new DelegateCommand(RemoveColorImpl);
         Colors = new ObservableCollection<Color>();

         SyncColors();
      }

      public override void OpenView()
      {
         var view = _lifetimeScope.Resolve<ColorRepeatEffectView>();
         view.DataContext = this;
         view.ShowDialog();
      }

      public override string Name => "Color Repeat Effect";

      public int Duration
      {
         get { return _colorRepeatEffect.ColorDurationMs; }
         set { _colorRepeatEffect.ColorDurationMs = value; }
      }

      public Color SelectedColor { get; set; }

      public int SelectedIndex { get; set; }

      public ICommand AddColor { get; }

      public ICommand RemoveColor { get; }

      public ObservableCollection<Color> Colors { get; }

      private void AddColorImpl()
      {
         _colorRepeatEffect.ColorSequence.Add(SelectedColor);
         SyncColors();
      }

      private void RemoveColorImpl()
      {
         if (SelectedIndex == -1)
            return;

         _colorRepeatEffect.ColorSequence.RemoveAt(SelectedIndex);
         SyncColors();
      }

      private void SyncColors()
      {
         Colors.Clear();
         foreach (var color in _colorRepeatEffect.ColorSequence)
            Colors.Add(color);
      }
   }
}
