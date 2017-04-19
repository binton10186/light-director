using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LightDirector.Infrastructure
{
   public class PopupContentControlDataContext : IInteractionRequestAware
   {
      private INotification _notification;
      private Action _finishInteraction;
      private object _controlContent;

      public Action FinishInteraction
      {
         get
         {            
            return GetContentDataContext()?.FinishInteraction;
         }
         set
         {
            _finishInteraction = value;
            if (GetContentDataContext() != null)
               GetContentDataContext().FinishInteraction = _finishInteraction;
         }
      }

      public INotification Notification
      {
         get
         {
            return GetContentDataContext().Notification;
         }

         set
         {
            _notification = value;
            if (GetContentDataContext() != null)
               GetContentDataContext().Notification = _notification;
         }
      }

      public void SetControlContent(object controlContent)
      {
         _controlContent = controlContent;
         Notification = _notification;
         FinishInteraction = _finishInteraction;
      }

      private IInteractionRequestAware GetContentDataContext()
      {
         return ((_controlContent as FrameworkElement)?.DataContext as IInteractionRequestAware);
      }
   }

   public class PopupContentControl : ContentControl
   {
      private PopupContentControlDataContext _dataContext;

      public PopupContentControl()
      {
         _dataContext = new PopupContentControlDataContext();
         _dataContext.SetControlContent(Content);
         DataContext = _dataContext;
      }

      protected override void OnContentChanged(object oldContent, object newContent)
      {
         base.OnContentChanged(oldContent, newContent);
         _dataContext.SetControlContent(newContent);
      }
   }
}
