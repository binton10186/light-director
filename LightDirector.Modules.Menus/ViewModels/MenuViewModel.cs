using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Modules.Menus.Services;
using LightDirector.Services;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System;

namespace LightDirector.Modules.Menus.ViewModels
{
   public class MenuViewModel
   {
      private readonly ProjectFileService _projectFileService;
      private readonly IEventAggregator _eventAggregator;
      private LightingPlan _lightingPlan;
      private ILightingPlanService _lightingPlanService;

      public MenuViewModel(
         ProjectFileService projectFileService, 
         IEventAggregator eventAggregator,
         ILightingPlanService lightingPlanService)
      {
         _projectFileService = projectFileService;
         _eventAggregator = eventAggregator;
         _lightingPlanService = lightingPlanService;

         NewLightingPlan = new DelegateCommand(CreateNewLightingPlan);
         OpenLightingPlan = new DelegateCommand(OpenLightingPlanImpl);
         ShowAbout = new DelegateCommand(ShowAboutImpl);
         SaveAs = new DelegateCommand(SaveAsImpl, () => _lightingPlan != null);
         CloseLightingPlan = new DelegateCommand(CloseLightingPlanImpl, () => _lightingPlan != null);
         Exit = new DelegateCommand(() => Application.Current.Shutdown());
         ShowStageTarget = new DelegateCommand(ExecuteShowStageTarget);

         ShowAboutRequest = new InteractionRequest<INotification>();

         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(OnLightingPlanChanged);
      }      

      public InteractionRequest<INotification> ShowAboutRequest { get; set; }

      public DelegateCommand NewLightingPlan { get; private set; }
      public DelegateCommand OpenLightingPlan { get; private set; }
      public DelegateCommand CloseLightingPlan { get; private set; }
      public DelegateCommand SaveAs { get; private set; }
      public DelegateCommand Exit { get; private set; }
      public DelegateCommand ShowStageTarget { get; }
      public DelegateCommand ShowAbout { get; }

      private void CreateNewLightingPlan()
      {
         SetLightingPlan(new LightingPlan());
      }

      private void OpenLightingPlanImpl()
      {
         var dialog = new OpenFileDialog { Filter = "Lighting Plan|*.lip" };
         var result = dialog.ShowDialog();
         if (result.HasValue && result.Value)
         {
            var lightingPlan = _projectFileService.Open(dialog.FileName);
            SetLightingPlan(lightingPlan);
         }
      }

      private void SaveAsImpl()
      {
         var dialog = new SaveFileDialog { Filter = "Lighting Plan|*.lip" };
         var result = dialog.ShowDialog();
         if (result.HasValue && result.Value)
            _projectFileService.SaveAs(_lightingPlan, dialog.FileName);
      }

      private void CloseLightingPlanImpl()
      {
         _lightingPlanService.CloseLightingPlan();
      }

      private void SetLightingPlan(LightingPlan lightingPlan)
      {
         _lightingPlanService.SetLightingPlan(lightingPlan);         
      }

      private void ShowAboutImpl()
      {
         ShowAboutRequest.Raise(new Notification { Title = "About Light Director" });
      }      

      private void ExecuteShowStageTarget()
      {
         var stageView = new StageWindow();
         stageView.Show();
      }

      private void OnLightingPlanChanged(LightingPlan lightingPlan)
      {
         _lightingPlan = lightingPlan;

         CloseLightingPlan.RaiseCanExecuteChanged();
         SaveAs.RaiseCanExecuteChanged();
      }
   }
}
