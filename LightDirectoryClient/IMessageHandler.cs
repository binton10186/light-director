using LightDirectorMessages;
using System;

namespace LightDirectoryClient
{
   public interface IMessageHandler : IDisposable
   {
      bool CanHandle(IMessage message);
      void Handle(IMessage message);
   }
}
