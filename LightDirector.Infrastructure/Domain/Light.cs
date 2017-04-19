using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.Domain
{
   public class Light
   {
      public Light()
      {
      }

      public event EventHandler LightChanged;

      public Light(Guid id, Guid specificationId, string name, Color color, Point3D position, Vector3D direction)
      {
         Id = id;
         Name = name;
         Color = color;
         Position = position;
         Direction = direction;
         SpecificationId = specificationId;
      }

      public Guid Id { get; set; }
      public string Name { get; set; }

      private Point3D _position;
      public Point3D Position
      {
         get
         {
            return _position;
         }
         set
         {
            _position = value;
            LightChanged?.Invoke(this, new EventArgs());
         }
      }

      private Vector3D _direction { get; set; }
      public Vector3D Direction
      {
         get
         {
            return _direction;
         }
         set
         {
            _direction = value;
            LightChanged?.Invoke(this, new EventArgs());
         }
      }

      private Color _color;
      public Color Color
      {
         get
         {
            return _color;
         }

         set
         {
            _color = value;
            LightChanged?.Invoke(this, new EventArgs());
         }
      }

      public int ChannelId { get; set; }

      public Guid SpecificationId { get; set; }
   }
}