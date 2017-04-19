namespace LightDirector.Domain
{
   public class DirectionControlType
   {
      private DirectionControlType(bool canSetDirection, bool canChangeDirection)
      {
         CanSetDirection = canSetDirection;
         CanChangeDirection = canChangeDirection;
      }

      public bool CanSetDirection { get; set; }

      public bool CanChangeDirection { get; set; }

      public static DirectionControlType Static()
      {
         return new DirectionControlType(true, false);
      }

      public static DirectionControlType Mover()
      {
         return new DirectionControlType(true, true);
      }
   }
}
