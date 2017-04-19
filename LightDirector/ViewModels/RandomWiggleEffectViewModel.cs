using Autofac;
using LightDirector.Domain;

namespace LightDirector.ViewModels
{
   public class RandomWiggleEffectViewModel : EffectViewModelBase
   {
      private readonly ILifetimeScope _lifetimeScope;

      public RandomWiggleEffectViewModel(RandomWiggleEffect effect, ILifetimeScope lifetimeScope) : base(effect)
      {
         _lifetimeScope = lifetimeScope;
      }

      public override string Name
      {
         get { return "Random Wiggle"; }
      }

      public override void OpenView()
      {
      }
   }
}
