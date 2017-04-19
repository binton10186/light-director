using LightDirector.Domain;
using LightDirector.Infrastructure.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightDirector.Infrastructure.Domain;
using LightDirector.Services;

namespace LightDirector.Modules.Viewer.ViewModels
{
   public class ViewerViewModel
   {
      private readonly IEventAggregator _eventAggregator;
      private readonly ILightingPlanService _lightingPlanService;

      public event Action<Light> LightAdded;
      public event Action<ColorEventArgs> LightColorChanged;
      public event Action<LightPositionChangedEventArgs> LightPositionChanged;
      public event Action<BrightnessEventArgs> LightBrightnessChanged;

      public ViewerViewModel(IEventAggregator eventAggregator, ILightingPlanService lightingPlanService)
      {
         _eventAggregator = eventAggregator;
         _lightingPlanService = lightingPlanService;
      }

      public void OnViewLoaded()
      {
         var lightingPlan = _lightingPlanService.GetLightingPlan();
         if (lightingPlan != null)
         {
            foreach (var light in lightingPlan.Lights)
               LightAdded?.Invoke(light);
         }

         _eventAggregator.GetEvent<LightAddedEvent>().Subscribe(AddLight, ThreadOption.UIThread);
         _eventAggregator.GetEvent<LightColorChangedEvent>().Subscribe(ChangeLightColor, ThreadOption.UIThread);
         _eventAggregator.GetEvent<LightBrightnessChangedEvent>().Subscribe(ChangeLightBrightness, ThreadOption.UIThread);
         _eventAggregator.GetEvent<LightPositionChangedEvent>().Subscribe(ChangeLightPosition, ThreadOption.UIThread);
      }

      private void ChangeLightPosition(LightPositionChangedEventArgs positionChangedArgs)
      {
         LightPositionChanged?.Invoke(positionChangedArgs);
      }

      private void ChangeLightColor(ColorEventArgs colorEventArgs)
      {
         LightColorChanged?.Invoke(colorEventArgs);
      }

      private void ChangeLightBrightness(BrightnessEventArgs brightnessEventArgs)
      {
         LightBrightnessChanged?.Invoke(brightnessEventArgs);
      }

      private void AddLight(Light light)
      {
         LightAdded?.Invoke(light);
      }
   }
}
