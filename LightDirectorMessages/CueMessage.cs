using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirectorMessages
{
   public class CueMessage : IMessage
   {
      public CueMessage()
      {
      }

      public CueMessage(string filename)
      {
         Filename = filename;
      }

      public string Filename { get; set; }
   }
}
