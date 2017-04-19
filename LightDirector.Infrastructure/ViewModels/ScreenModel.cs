using LightDirector.Infrastructure.Services;
using LightDirector.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LightDirector.ViewModels
{

   public class ScreenModel
   {
      private readonly ICueTimeService _cueTimeService;
      private readonly MediaElement _mediaElement;
      private readonly IModeService _modeService;

      public ScreenModel(Point leftPoint, Point rightPoint, double height, ICueTimeService cueTimeService, IModeService modeService)
      {
         _modeService = modeService;
         _cueTimeService = cueTimeService;
         _mediaElement = new MediaElement {LoadedBehavior = MediaState.Manual};
         _mediaElement.ScrubbingEnabled = true;
         _mediaElement.Volume = 0;
         Model = CreateScreen(_mediaElement, leftPoint, rightPoint, height);

         _cueTimeService.TimeStopped += (o, a) =>
         {
            if(!modeService.IsLive)
               _mediaElement.Pause();
         };

         _cueTimeService.TimeStarted += (o, a) =>
         {
            if(!modeService.IsLive)
               _mediaElement.Play();
         };

         _cueTimeService.TimeJumped += (o, a) =>
         {
            if(!modeService.IsLive)
               _mediaElement.Position = a.CueTimeElapsed;
         };
      }

      public ModelVisual3D Model { get; private set; }

      public void Play(string fileName)
      {
         if (!_modeService.IsLive)
         {
            var fileUri = new Uri(fileName);
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
               {
                  if (_mediaElement.Source != fileUri)
                  {
                     _mediaElement.Source = fileUri;
                     _mediaElement.Play();
                  }
               }),
               System.Windows.Threading.DispatcherPriority.Send);
         }
      }

      public void Stop()
      {
         _mediaElement.Stop();
         _mediaElement.Source = null;
      }

      private static ModelVisual3D CreateScreen(
         Visual mediaElement, Point leftPoint, Point rightPoint, double height)
      {
         return new ModelVisual3D
         {
            Content = new GeometryModel3D
            {
               Geometry = new MeshGeometry3D
               {
                  Positions = new Point3DCollection(new[]
                  {
                     new Point3D {X = leftPoint.X, Y = 0, Z = leftPoint.Y},
                     new Point3D {X = rightPoint.X, Y = 0, Z = rightPoint.Y},
                     new Point3D {X = rightPoint.X, Y = height, Z = rightPoint.Y},
                     new Point3D {X = leftPoint.X, Y = height, Z = leftPoint.Y},
                  }),
                  TextureCoordinates = new PointCollection(new[]
                  {
                     new Point(leftPoint.X, height),
                     new Point(rightPoint.X, height),
                     new Point(rightPoint.X, 0),
                     new Point(leftPoint.X, 0),
                  }),
                  TriangleIndices = new Int32Collection(new[]
                  {
                     0, 1, 2, 0, 2, 3
                  }),
                  Normals = new Vector3DCollection(new[]
                  {
                     new Vector3D {X = 0, Y = 0, Z = 0},
                     new Vector3D {X = 0, Y = 0, Z = 0},
                     new Vector3D {X = 0, Y = 0, Z = 0},
                     new Vector3D {X = 0, Y = 0, Z = 0},
                     new Vector3D {X = 0, Y = 0, Z = 0},
                     new Vector3D {X = 0, Y = 0, Z = 0}
                  })
               },
               Material = new MaterialGroup
               {
                  Children = new MaterialCollection(new Material[]
                  {
                     new DiffuseMaterial
                     {
                        Brush = Brushes.Black
                     },
                     new EmissiveMaterial
                     {
                        Brush = new VisualBrush
                        {
                           Visual = mediaElement
                        },
                     }
                  })
               }
            }
         };
      }
   }
}