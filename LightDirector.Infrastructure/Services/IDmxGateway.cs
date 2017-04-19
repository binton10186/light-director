namespace LightDirector
{
   public interface IDmxGateway
   {
      void Start();
      void Stop();
      void SetChannelValue(int channel, short data);
      short GetChannelValue(int channel);
   }
}