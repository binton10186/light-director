using Autofac;
using LightDirector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.ViewModels
{
   class MovingSpotEffectViewModel : EffectViewModelBase
   {
      private readonly ILifetimeScope _lifetimeScope;

      public MovingSpotEffectViewModel(MovingSpotEffect effect, ILifetimeScope lifetimeScope) : base(effect)
      {
         _lifetimeScope = lifetimeScope;
      }

      public override string Name
      {
         get { return "Moving Spot"; }
      }

      public override void OpenView()
      {
      }
   }
}
