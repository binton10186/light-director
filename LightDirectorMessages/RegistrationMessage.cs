using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirectorMessages
{
   public class RegistrationMessage : IMessage
   {
      public RegistrationMessage()
      {
      }

      public RegistrationMessage(string name)
      {
         Name = name;
      }

      public string Name { get; set; }
   }
}
