using System;
using LightDirector.Domain;
using LightDirector.Infrastructure.Domain;

namespace LightDirector.ViewModels
{
   public class RunCueViewModel
   {
      private readonly ICue _cue;

      public RunCueViewModel(ICue cue)
      {
         _cue = cue;
      }

      public Guid Id { get { return _cue.Id; } }

      public string Name { get { return _cue.Name; } }
   }
}