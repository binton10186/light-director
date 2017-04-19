using System;
using LightDirector.Domain;
using LightDirector.Services;
using LightDirectorMessages;
using WebSocketSharp.Server;
using System.Linq;

namespace LightDirector
{
   public interface IClientGateway
   {
      void Start();
      void Stop();

      void Send(Client client, IMessage message);
   }
   

   public class ClientGateway : IClientGateway
   {
      private readonly IClientRepository _clientRepository;
      private LightDirectorBehavior _lightDirectorBehavior;
      private readonly WebSocketServer _webSocketServer;

      public event EventHandler<MessageEventArgs> MessageReady;

      public ClientGateway(IClientRepository clientRepository)
      {
         _clientRepository = clientRepository;
         try
         {
            _webSocketServer = new WebSocketServer("ws://192.168.1.65:11997");
         }
         catch
         {
            _webSocketServer = new WebSocketServer("ws://localhost:11997");
         }
         

         _lightDirectorBehavior = new LightDirectorBehavior(_clientRepository, this);
      }

      public void Send(Client client, IMessage message)
      {
         var args = new MessageEventArgs(message, client.SessionId);
         var evt = MessageReady;
         if (evt != null)
            evt(this, args);
      }

      public void Start()
      {
         _webSocketServer.KeepClean = false;
         _webSocketServer.AddWebSocketService("/LightDirector", () => new LightDirectorBehavior(_clientRepository, this));
         _webSocketServer.Start();
      }

      public void Stop()
      {
         _webSocketServer.Stop();
      }      
   }

   public class MessageEventArgs : EventArgs
   {
      public MessageEventArgs(IMessage message, string sessionId)
      {
         Message = message;
         SessionId = sessionId;
      }

      public IMessage Message { get; }

      public string SessionId { get; }
   }
}
