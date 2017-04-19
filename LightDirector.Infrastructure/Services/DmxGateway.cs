using System.Runtime.InteropServices;

namespace LightDirector
{
   public class DmxGateway : IDmxGateway
   {
      private readonly short[] _channels;
      private const int ChannelCount = 512;

      [DllImport("k8062d.dll")]
      public static extern void StartDevice();

      [DllImport("k8062d.dll")]
      public static extern void SetData(int channel, int data);

      [DllImport("k8062d.dll")]
      public static extern void SetChannelCount(int count);

      [DllImport("k8062d.dll")]
      public static extern void StopDevice();

      public DmxGateway()
      {
         SetChannelCount(ChannelCount);
         _channels = new short[ChannelCount];
      }

      public void Start()
      {
         StartDevice();
         SetChannelCount(ChannelCount);
         for (var i = 0; i < ChannelCount; i++)
            SetChannelValue(i, 0);
      }

      public void Stop()
      {
         StopDevice();
      }

      public void SetChannelValue(int channel, short data)
      {
         if (GetChannelValue(channel) != data)
         {
            SetData(channel, data);
            _channels[channel] = data;
         }
      }

      public short GetChannelValue(int channel)
      {
         return _channels[channel];
      }
   }
}
