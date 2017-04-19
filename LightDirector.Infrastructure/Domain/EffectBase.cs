using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace LightDirector.Domain
{
   [Serializable]
   [XmlInclude(typeof(ColorRepeatEffect))]
   [XmlInclude(typeof(MovingSpotEffect))]
   [XmlInclude(typeof(RandomWiggleEffect))]
   [XmlInclude(typeof(SwingingLightEffect))]
   [XmlInclude(typeof(FigureOfEightEffect))]
   public abstract class EffectBase
   {
      [XmlIgnore]
      public abstract long StartMs { get; set; }

      [XmlIgnore]
      public abstract long EndMs { get; set; }

      public abstract bool IsFor(LightProperty property);

      public abstract Color GetColor(TimeSpan time);

      public abstract Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService);

      public abstract int GetBrightness(TimeSpan time);
   }
}
