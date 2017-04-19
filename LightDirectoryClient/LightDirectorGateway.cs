using LightDirectorMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using System.Configuration;

namespace LightDirectoryClient
{
   public class LightDirectorGateway : IDisposable
   {
      private readonly IEnumerable<IMessageHandler> _messageHandlers;
      private WebSocket _webSocket;
      private object _connectionLock = new object();

      public LightDirectorGateway(IEnumerable<IMessageHandler> messageHandlers)
      {
         _messageHandlers = messageHandlers;

         var serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
         _webSocket = new WebSocket(serverUrl);

         _webSocket.OnMessage += (sender, e) =>
         {
            var message = (IMessage)JsonConvert.DeserializeObject(e.Data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            HandleMessage(message);
         };

         _webSocket.OnClose += (sender, e) =>
         {
            HandleMessage(new CueMessage(null));
            Task.Factory.StartNew(Connect);
         };

         _webSocket.OnError += (sender, e) =>
         {
            HandleMessage(new CueMessage(null));
            Task.Factory.StartNew(Connect);
         };
      }

      private void HandleMessage(IMessage message)
      {
         foreach (var messageHandler in _messageHandlers)
         {
            if (messageHandler.CanHandle(message))
               messageHandler.Handle(message);
         }
      }

      public void Connect()
      {
         lock(_connectionLock)
         {
            if (_webSocket.ReadyState == WebSocketState.Open)
               return;

            _webSocket.Connect();
            var screenName = ConfigurationManager.AppSettings["ClientName"];
            var message = new RegistrationMessage(screenName);
            var serialized = JsonConvert.SerializeObject(message, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            _webSocket.SendAsync(serialized);
         }
      }

      public void Dispose()
      {
         _webSocket.Dispose();

         foreach (var handler in _messageHandlers)
            handler.Dispose();
      }
   }
}
