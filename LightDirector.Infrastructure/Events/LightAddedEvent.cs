using Prism.Events;
using LightDirector.Domain;

namespace LightDirector.Infrastructure.Events
{
   public class LightAddedEvent : PubSubEvent<Light>
   {
   }
}
