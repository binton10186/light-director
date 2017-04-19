using LightDirector.Domain;
using System;

namespace LightDirector.ViewModels
{
   public class FigureOfEightViewModel : EffectViewModelBase
   {
      public FigureOfEightViewModel(FigureOfEightEffect effect) : base(effect)
      {
      }

      public override string Name => "Figure Of Eight";

      public override void OpenView()
      {
         throw new NotImplementedException();
      }
   }
}
