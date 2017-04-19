using LightDirector.Domain;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;

namespace LightDirector.Services
{
   public interface IClientRepository
   {
      void Add(Client client);

      Client RetrieveSingleOrDefault(string name);
   }

   public class ClientRepository : IClientRepository
   {
      private readonly ConcurrentDictionary<string, Client> _clients = new ConcurrentDictionary<string, Client>();
      
      public void Add(Client client)
      {
         _clients[client.Name] = client;
      }

      public Client RetrieveSingleOrDefault(string name)
      {
         Client client = null;
         _clients.TryGetValue(name, out client);
         return client;
      }
   }
}
