using System;
using System.Windows.Media.Media3D;

namespace LightDirector.ViewModels
{
   public class SceneChangedEventArgs : EventArgs
   {
      public SceneChangedEventArgs(ModelVisual3D scene)
      {
         Scene = scene;
      }

      public ModelVisual3D Scene { get; private set; }
   }
}