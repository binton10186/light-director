using LightDirector.Infrastructure.Domain;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.Infrastructure.Events
{
   public class LightDirectionChangedEvent : PubSubEvent<DirectionEventArgs>
   {
   }
}
