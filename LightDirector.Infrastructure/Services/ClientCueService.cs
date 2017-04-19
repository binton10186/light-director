using LightDirector.Infrastructure.Domain;
using LightDirector.Infrastructure.Events;
using LightDirectorMessages;
using Prism.Events;
using System.Threading.Tasks;

namespace LightDirector.Services
{
   public interface IClientCueService
   {
   }

   public class ClientCueService : IClientCueService
   {
      private readonly IClientGateway _clientGateway;
      private readonly IClientRepository _clientRepository;
      private readonly IEventAggregator _eventAggregator;

      public ClientCueService(
         IClientRepository clientRepository,
         IClientGateway clientGateway,
         IEventAggregator eventAggregator)
      {
         _clientRepository = clientRepository;
         _clientGateway = clientGateway;
         _eventAggregator = eventAggregator;

         _eventAggregator.GetEvent<SelectedCueChangedEvent>().Subscribe(OnSelectedCueChanged);
      }

      private void OnSelectedCueChanged(ICue cue)
      {
         if (cue == null)
            return;

         Task.Factory.StartNew(() =>
         {
            SetLeftScreen(cue.LeftVideoFileName);
         });

         Task.Factory.StartNew(() =>
         {
            SetCenterScreen(cue.CenterVideoFileName);
         });

         Task.Factory.StartNew(() =>
         {
            SetRightScreen(cue.RightVideoFileName);
         });
      }

      private void SetLeftScreen(string filename)
      {
         SetScreen("LeftScreen", filename);
      }

      private void SetCenterScreen(string filename)
      {
         SetScreen("CenterScreen", filename);
      }

      private void SetRightScreen(string filename)
      {
         SetScreen("RightScreen", filename);
      }

      private void SetScreen(string screenName, string filename)
      {
         var client = _clientRepository.RetrieveSingleOrDefault(screenName);
         if (client != null)
         {
            var message = new CueMessage(filename);
            _clientGateway.Send(client, message);
         }
      }
   }
}
