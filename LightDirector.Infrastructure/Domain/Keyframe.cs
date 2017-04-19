using System;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   [XmlInclude(typeof(BrightnessKeyframe))]
   [XmlInclude(typeof(ColorKeyframe))]
   [XmlInclude(typeof(DirectionKeyframe))]
   [XmlInclude(typeof(ChannelKeyframe))]
   public abstract class Keyframe
   {
      [XmlIgnore]
      public abstract TimeSpan Time { get; set; }
   }
}
