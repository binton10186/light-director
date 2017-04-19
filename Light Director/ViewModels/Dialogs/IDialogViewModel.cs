using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.ViewModels.Dialogs
{
   public interface IDialogViewModel<T>
   {
      string Title { get; }

      T GetResult();
   }
}
