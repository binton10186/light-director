using LightDirector.Infrastructure.Domain;
using Prism.Events;

namespace LightDirector.Infrastructure.Events
{
   public class LightColorChangedEvent : PubSubEvent<ColorEventArgs>
   {
   }
}
