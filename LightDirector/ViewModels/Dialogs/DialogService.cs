using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDirector.ViewModels.Dialogs
{
   public class DialogService
   {
      private IComponentContext _componentContext;

      public DialogService(IComponentContext componentContext)
      {
         _componentContext = componentContext;
      }

      public TResult Show<TViewModel, TResult>() where TViewModel : IDialogViewModel<TResult>
      {
         var viewModel = _componentContext.Resolve<TViewModel>();

         return viewModel.GetResult();
      } 
   }
}
