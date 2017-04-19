using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.Domain
{
   public class Client
   {
      public Client(string sessionId, string name)
      {
         SessionId = sessionId;
         Name = name;
      }

      public string SessionId { get; }
      public string Name { get; }
   }
}
