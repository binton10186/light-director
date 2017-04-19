namespace LightDirector.ViewModels
{
   public class RunLightViewModel
   {
      private readonly Domain.Light _light;

      public RunLightViewModel(Domain.Light light)
      {
         _light = light;
      }
   }
}