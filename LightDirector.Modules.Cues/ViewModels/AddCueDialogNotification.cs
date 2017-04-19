using LightDirector.Infrastructure.Domain;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace LightDirector.Modules.Cues.ViewModels
{
   public class AddCueDialogNotification : Notification
   {
      public bool Success { get; set; }

      public string Name { get; set; }

      public IEnumerable<ICue> ReferenceCues {get; set;}

      public ICue ReferenceCue { get; set; }
   }
}