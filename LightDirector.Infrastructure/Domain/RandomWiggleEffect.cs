using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   public class RandomWiggleEffect : EffectBase
   {
      private Vector3D _currentDirection = new Vector3D(0, -1, 0);
      private int _seed;

      private Random _directionChangeRandom;
      private Random _randomX;
      private Random _randomY;
      private Random _randomZ;
      private const int DirectionChangeFactor = 15;
      private TimeSpan _targetTime;
      private Vector3D _stepping = new Vector3D(0, 0, 0);
      private Point3D _originalSpot = new Point3D(0, 0, 0);

      public int Seed
      {
         get { return _seed; }
         set
         {
            _seed = value;
            _randomX = new Random(value);
            _randomY = new Random(value + 1);
            _randomZ = new Random(value + 2);
            _directionChangeRandom = new Random(value + 3);
         }
      }

      public override long EndMs { get; set; }

      public override long StartMs { get; set; }

      public override int GetBrightness(TimeSpan time)
      {
         throw new InvalidOperationException();
      }

      public override Color GetColor(TimeSpan time)
      {
         throw new InvalidOperationException();
      }

      public override Vector3D GetDirection(TimeSpan time, Light light, IStagePositionService stagePositionService)
      {
         var changeDirection = _directionChangeRandom.Next(0, DirectionChangeFactor) == DirectionChangeFactor - 1;

         if (changeDirection)
         {
            var directionSign = new Random();
            var x = _randomX.Next(-2000, 9000) / 1000d;
            var z = _randomZ.Next(-1000, 4000) / 1000d;
            
            var targetSpot = new Point3D(x, 0, z);
            var currentSpot = _currentDirection + light.Position;
            _originalSpot = currentSpot;
            _stepping = (targetSpot - currentSpot)/500;
            var factor = 0.004 / _stepping.Length;
            _stepping = _stepping * factor;
            _targetTime = time;
         }

         var steppingCount = (time - _targetTime).TotalMilliseconds;
         steppingCount = steppingCount > 500 ? 500 : steppingCount;
         _currentDirection = (_originalSpot + steppingCount * _stepping) - light.Position;
         return _currentDirection;
      }

      public override bool IsFor(LightProperty property)
      {
         return property == LightProperty.Direction;
      }
   }
}
