using System;
using LightDirector.Domain;
using LightDirectorMessages;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace LightDirector.Services
{
   public class LightDirectorBehavior : WebSocketBehavior
   {
      private IClientRepository _clientRepository;

      public LightDirectorBehavior()
      {
      }

      public LightDirectorBehavior(IClientRepository clientRepository, ClientGateway gateway)
      {
         _clientRepository = clientRepository;
         gateway.MessageReady += OnMessageReady;
      }      

      protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
      {
         base.OnMessage(e);

         var message = (IMessage)JsonConvert.DeserializeObject(e.Data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
         if (message is RegistrationMessage)
         {
            var registrationMessage = (RegistrationMessage)message;
            var client = new Client(base.ID, registrationMessage.Name);
            _clientRepository.Add(client);
         }
      }

      protected override void OnError(ErrorEventArgs e)
      {
         base.OnError(e);
         Console.WriteLine($"{e.Message} - {e.Exception}");
      }

      protected override void OnClose(CloseEventArgs e)
      {
         base.OnClose(e);
      }

      protected override void OnOpen()
      {
         base.OnOpen();
      }

      private void OnMessageReady(object sender, MessageEventArgs e)
      {
         //Sessions.SendToAsync(e.SessionId, Serialize(e.Message));
         if (ID == e.SessionId)
         {
            var serialized = Serialize(e.Message);
            Send(serialized);
         }
      }

      private string Serialize(IMessage message)
      {
         return JsonConvert.SerializeObject(message, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
      }
   }
}
