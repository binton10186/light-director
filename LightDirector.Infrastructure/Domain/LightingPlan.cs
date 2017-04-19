using LightDirector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LightDirector.Infrastructure.Domain
{
   [Serializable]   
   public class LightingPlan
   {
      public LightingPlan()
      {
         Name = "New Lighting Plan";
         IsAmbientLightOn = true;

         Cues = new List<ICue>();
         ReferenceCues = new List<ICue>();
         Lights = new List<Light>();
      }

      public event EventHandler LightsChanged;

      public string Name { get; set; }

      public string SceneFileName { get; set; }

      [XmlArray("Cues")]      
      public List<ICue> Cues { get; set; }

      [XmlArray("ReferenceCues")]
      public List<ICue> ReferenceCues { get; set; }

      public void AddCue(int index, ICue lightingCue)
      {
         Cues.Insert(index, lightingCue);
      }

      public void AddReferenceCue(int index, ICue cue)
      {
         ReferenceCues.Insert(index, cue);
      }

      public bool IsAmbientLightOn { get; set; }

      private List<Light> _lights;

      [XmlArray("Lights")]
      public List<Light> Lights
      {
         get { return _lights; }
         private set
         {
            if(_lights != null)
            {
               foreach (var light in _lights)
                  light.LightChanged -= OnLightChanged;
            }

            _lights = value;            
         }
      }

      public void Initialize()
      {
         if (_lights != null)
         {
            foreach (var light in _lights)
               light.LightChanged += OnLightChanged;
         }

         foreach(var cue in Cues)
         {
            var referenceCue = cue as ReferenceCue;
            if(referenceCue != null)
            {
               referenceCue.Underlying = ReferenceCues.Single(c => c.Id == referenceCue.UnderlyingId);
            }
         }
      }

      public void AddLight(Light light)
      {
         Lights.Add(light);
         LightsChanged?.Invoke(this, new EventArgs());
         light.LightChanged += (_, __) => LightsChanged?.Invoke(this, new EventArgs());
      }

      public ICue GetCueById(Guid id)
      {
         var cue = Cues.SingleOrDefault(c => c.Id == id);
         var referenceCue = ReferenceCues.SingleOrDefault(c => c.Id == id);
         return cue ?? referenceCue;
      }

      private void OnLightChanged(object sender, EventArgs e)
      {
         LightsChanged?.Invoke(this, new EventArgs());
      }

      public void RemoveCue(Guid id)
      {
         var cue = Cues.Single(c => c.Id == id);
         Cues.Remove(cue);
      }
   }
}
