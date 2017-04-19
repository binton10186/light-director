using System;
using System.Linq;
using LightDirector.Domain;
using LightDirector.Services;
using LightDirector.Infrastructure.Domain;
using Prism.Events;
using LightDirector.Infrastructure.Events;

namespace LightDirector
{
   public class DmxService : IDmxService
   {
      private readonly IDmxGateway _gateway;
      private readonly LightSpecificationRepository _lightSpecificationRepository;
      private readonly ILightingPlanService _lightingPlanService;

      public DmxService(IEventAggregator eventAggregator, IDmxGateway gateway, LightSpecificationRepository lightSpecificationRepository, ILightingPlanService lightingPlanService)
      {
         _gateway = gateway;
         _lightSpecificationRepository = lightSpecificationRepository;
         _lightingPlanService = lightingPlanService;

         eventAggregator.GetEvent<LightChannelValueChangedEvent>().Subscribe(LightChannelValueChanged);
         eventAggregator.GetEvent<LightBrightnessChangedEvent>().Subscribe(LightBrightnessChanged);
         eventAggregator.GetEvent<LightDirectionChangedEvent>().Subscribe(LightDirectionChanged);
         eventAggregator.GetEvent<LightColorChangedEvent>().Subscribe(LightColorChanged);
      }         

      private void LightChannelValueChanged(DmxChannelValue channelValue)
      {
         var light = _lightingPlanService.GetLightingPlan().Lights.SingleOrDefault(l => l.Id == channelValue.LightId);
         if (light == null)
            return;

         var specification = _lightSpecificationRepository.RetrieveById(light.SpecificationId);
         if (specification == null)
            return;

         var channel = light.ChannelId + channelValue.ChannelId - 1;
         _gateway.SetChannelValue(channel, Convert.ToInt16(channelValue.Value));
      }

      private void LightDirectionChanged(DirectionEventArgs e)
      {
         var light = _lightingPlanService.GetLightingPlan().Lights.SingleOrDefault(l => l.Id == e.LightId);
         if (light == null)
            return;

         var specification = _lightSpecificationRepository.RetrieveById(light.SpecificationId);
         if (specification == null)
            return;

         if (!specification.DirectionControlType.CanChangeDirection)
            return;

         var tiltChannel = light.ChannelId + specification.TiltChannel - 1;
         var panChannel = light.ChannelId + specification.PanChannel - 1;

         var panAndTilt = CoordinateSystemConverter.ConvertToSphericalVector(e.Direction);

         var panAngle = light.Id == new Guid("8415ad81-0602-4660-9860-7b6172f8cc89") ? panAndTilt.Pan * -1 : panAndTilt.Pan;

         var pan = AngleToByte(panAngle, specification.MinPanAngle, specification.MaxPanAngle);
         var tilt = AngleToByte(panAndTilt.Tilt, specification.MinTiltAngle, specification.MaxTiltAngle);

         _gateway.SetChannelValue(tiltChannel, tilt);
         _gateway.SetChannelValue(panChannel, pan);
      }

      private void LightColorChanged(ColorEventArgs e)
      {
         var light = _lightingPlanService.GetLightingPlan().Lights.SingleOrDefault(l => l.Id == e.LightId);
         if (light == null)
            return;

         var specification = _lightSpecificationRepository.RetrieveById(light.SpecificationId);
         if (specification == null)
            return;

         if (!specification.ColorControlType.HasRgbCapability)
            return;

         var redChannel = light.ChannelId + specification.RedChannel - 1;
         var greenChannel = light.ChannelId + specification.GreenChannel - 1;
         var blueChannel = light.ChannelId + specification.BlueChannel - 1;

         _gateway.SetChannelValue(redChannel, e.Color.R);
         _gateway.SetChannelValue(greenChannel, e.Color.G);
         _gateway.SetChannelValue(blueChannel, e.Color.B);
      }

      private void LightBrightnessChanged(BrightnessEventArgs e)
      {
         var light = _lightingPlanService.GetLightingPlan().Lights.SingleOrDefault(l => l.Id == e.LightId);
         if (light == null)
            return ;

         var specification = _lightSpecificationRepository.RetrieveById(light.SpecificationId);
         if (specification == null)
            return;

         if (!specification.HasBrightnessControl)
            return;

         var brightnessChannel = light.ChannelId + specification.BrightnessChannel - 1;
         var brightness = BrightnessToByte(e.Brightness);

         _gateway.SetChannelValue(brightnessChannel, brightness);
      }

      private short BrightnessToByte(int brightness)
      {
         var channelBrightness = 255.0 / 100 * brightness;
         channelBrightness = (channelBrightness < 0) ? (short)0 : channelBrightness;
         channelBrightness = (channelBrightness > 255) ? (short)255 : channelBrightness;
         var channelValue = Convert.ToInt16(channelBrightness);
         return channelValue;
      }

      private short AngleToByte(decimal angle, decimal minAngle, decimal maxAngle)
      {
         angle = angle < minAngle ? minAngle : angle;
         angle = angle > maxAngle ? maxAngle : angle; 

         var numerator = angle - minAngle;
         var denominator = maxAngle - minAngle;

         var decimalValue = numerator / denominator * 255;
         var value = Convert.ToInt16(decimalValue);

         value = value < 0 ? (short)0 : value;
         value = value > 255 ? (short)255 : value;
         return value;
      }

      public void Start()
      {
         ClearAllChannels();
         _gateway.Start();
      }

      public void Stop()
      {
         ClearAllChannels();
         _gateway.Stop();
      }

      private void ClearAllChannels()
      {
         for (var i = 0; i < 512; i++)
            _gateway.SetChannelValue(i, 0);
      }
   }   
}
