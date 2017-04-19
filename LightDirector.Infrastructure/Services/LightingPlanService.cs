using LightDirector.Infrastructure.Domain;
using Prism.Events;
using LightDirector.Infrastructure.Events;

namespace LightDirector.Services
{
   public interface ILightingPlanService
   {
      LightingPlan GetLightingPlan();

      void SetLightingPlan(LightingPlan lightingPlan);

      void CloseLightingPlan();
   }

   public class LightingPlanService : ILightingPlanService
   {
      private readonly IEventAggregator _eventAggregator;
      private LightingPlan _lightingPlan;

      public LightingPlanService(IEventAggregator eventAggregator)
      {
         _eventAggregator = eventAggregator;
      }

      public void CloseLightingPlan()
      {
         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Publish(null);
      }

      public LightingPlan GetLightingPlan()
      {
         return _lightingPlan;
      }

      public void SetLightingPlan(LightingPlan lightingPlan)
      {
         _lightingPlan = lightingPlan;
         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Publish(lightingPlan);
      }
   }
}
