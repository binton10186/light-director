using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   [Serializable]
   public class ParCanSetting
   {
      public ParCanSetting()
      {
      }

      public ParCanSetting(Guid id, int brightness, Color color, Vector3D direction, IEnumerable<DmxChannelValue> channelValues)
      {
         Id = id;
         Brightness = brightness;
         Color = color;
         Direction = direction;
         ChannelValues = channelValues;
      }

      public Guid Id { get; }

      public int Brightness { get; }

      public Color Color { get; }

      public Vector3D Direction { get; }

      public IEnumerable<DmxChannelValue> ChannelValues { get; }
   }
}