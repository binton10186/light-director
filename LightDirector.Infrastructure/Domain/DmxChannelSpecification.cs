namespace LightDirector.Infrastructure.Domain
{
   public class DmxChannelSpecification
   {
      public DmxChannelSpecification(int channelId, string name)
      {
         ChannelId = channelId;
         Name = name;
      }

      public int ChannelId { get; }

      public string Name { get; }
   }
}
