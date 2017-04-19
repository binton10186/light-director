using LightDirector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.ViewModels
{
   public class SwingingLightEffectViewModel : EffectViewModelBase
   {
      public SwingingLightEffectViewModel(SwingingLightEffect effect) : base(effect)
      {
      }

      public override string Name => "Swinging Light";

      public override void OpenView()
      {
         throw new NotImplementedException();
      }
   }
}
