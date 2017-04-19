using LightDirector.Domain;
using Prism.Mvvm;

namespace LightDirector.ViewModels
{
   public abstract class EffectViewModelBase : BindableBase
   {
      private readonly EffectBase _effect;

      public EffectViewModelBase(EffectBase effect)
      {
         _effect = effect;
      }

      public long StartTime
      {
         get { return _effect.StartMs; }
         set { _effect.StartMs = value; }
      }

      public long EndTime
      {
         get { return _effect.EndMs; }
         set { _effect.EndMs = value; }
      }

      public abstract void OpenView();

      public abstract string Name { get; }
   }
}
