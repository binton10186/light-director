using GongSolutions.Wpf.DragDrop;
using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirector.Infrastructure.Services;
using LightDirector.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LightDirector.Modules.Cues.ViewModels
{
   public class CuesViewModel : BindableBase, IDropTarget
   {
      private readonly object _selectedCueLock = new object();
      private readonly ICueTimeService _cueTimeService;
      private readonly ILightingPlanService _lightingPlanService;
      private readonly Func<ICue, CueViewModel> _cueViewModelFactory;

      private CueViewModel _selectedCue;
      private LightingPlan _lightingPlan;
      private IEventAggregator _eventAggregator;

      public CuesViewModel(
         ICueTimeService cueTimeService, 
         ILightingPlanService lightingPlanService,
         IEventAggregator eventAggregator,
         Func<ICue, CueViewModel> cueViewModelFactory)
      {
         _cueTimeService = cueTimeService;
         _lightingPlanService = lightingPlanService;
         _cueViewModelFactory = cueViewModelFactory;
         _eventAggregator = eventAggregator;

         RecordNewCue = new DelegateCommand(RecordNewCueImpl);
         Cues = new ObservableCollection<CueViewModel>();

         ShowAddCueRequest = new InteractionRequest<INotification>();

         _eventAggregator.GetEvent<LightingPlanChangedEvent>().Subscribe(LightingPlanChanged);
      }      

      public ICommand RecordNewCue { get; }

      public InteractionRequest<INotification> ShowAddCueRequest { get; }

      public ObservableCollection<CueViewModel> Cues { get; }

      public CueViewModel SelectedCue
      {
         get
         {
            lock (_selectedCueLock)
            {
               return _selectedCue;
            }
         }
         set
         {
            lock (_selectedCueLock)
            {
               _selectedCue = value;
               if (_selectedCue != null)
               {
                  _eventAggregator.GetEvent<SelectedCueChangedEvent>().Publish(_selectedCue.Cue);
                  _cueTimeService.SetTime(TimeSpan.Zero);
               }
               else
               {
                  _eventAggregator.GetEvent<SelectedCueChangedEvent>().Publish(null);
               }
            }
            OnPropertyChanged();
         }
      }

      public void RemoveSelectedCue()
      {
         if (_lightingPlan == null) return;

         _lightingPlan.RemoveCue(_selectedCue.Id);
         var vm = Cues.Single(c => c.Id == _selectedCue.Id);
         Cues.Remove(vm);
      }

      public void DragOver(IDropInfo dropInfo)
      {
         var sourceItem = dropInfo.Data as CueViewModel;
         var targetItem = dropInfo.TargetItem as CueViewModel;

         if (sourceItem != null && targetItem != null)
         {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Copy;
         }
      }

      public void Drop(IDropInfo dropInfo)
      {
         if (_lightingPlan == null) return;

         var currentCue = (CueViewModel)dropInfo.Data;
         var currentIndex = Cues.IndexOf(currentCue);
         Cues.Move(currentIndex, dropInfo.InsertIndex);
         _lightingPlan.Cues = Cues.Select(c => c.Cue).ToList();
      }

      private void RecordNewCueImpl()
      {
         if (_lightingPlan == null) return;

         var addCueDialogNotification = new AddCueDialogNotification { Title = "Add Cue", ReferenceCues = _lightingPlan.ReferenceCues };
         ShowAddCueRequest.Raise(addCueDialogNotification);

         if (addCueDialogNotification.Success)
         {
            ICue cue = null;
            if (addCueDialogNotification.ReferenceCue == null)
               cue = new Domain.Cue(addCueDialogNotification.Name);
            else
               cue = new Domain.ReferenceCue(addCueDialogNotification.Name, addCueDialogNotification.ReferenceCue);

            var insertIndex = SelectedCue == null ? Cues.Count : Cues.IndexOf(SelectedCue) + 1;
            _lightingPlan.AddCue(insertIndex, cue);
            var cueViewModel = _cueViewModelFactory(cue);
            Cues.Insert(insertIndex, cueViewModel);
         }
      }

      private void LightingPlanChanged(LightingPlan lightingPlan)
      {
         _lightingPlan = lightingPlan;
         Cues.Clear();

         if (lightingPlan != null)
         {
            foreach (var cueViewModel in CreateCueViewModels(lightingPlan.Cues))
               Cues.Add(cueViewModel);
         }
      }

      private IEnumerable<CueViewModel> CreateCueViewModels(IEnumerable<ICue> cues)
      {
         var cueViewModels = cues.Select(c => _cueViewModelFactory(c));
         return cueViewModels;
      }
   }
}
