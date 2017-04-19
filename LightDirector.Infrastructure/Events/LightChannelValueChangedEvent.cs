using LightDirector.Domain;
using Prism.Events;

namespace LightDirector.Infrastructure.Events
{
   public class LightChannelValueChangedEvent : PubSubEvent<DmxChannelValue>
   {
   }
}
