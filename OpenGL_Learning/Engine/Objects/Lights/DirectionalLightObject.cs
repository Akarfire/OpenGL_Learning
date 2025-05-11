using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Mathematics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Objects.Lights
{
    internal class DirectionalLightObject: LightObject
    {
        // Whether this light should be considered the main light in the current world
        // If true: this light's direction will be assumed to be the world light direction
        public bool mainLight = true;

        public DirectionalLightObject(Engine inEngine, float intensity, Color color, float softness) : base(inEngine) 
        {
            lightData.intensity = intensity;
            lightData.type = 1;
            lightData.softness = softness;

            SetLightColor(color);

            lightData.lightColor = new Vector3(1f, 1f, 1f);
        }

        public override void OnUpdated(float deltaTime)
        {
            base.OnUpdated(deltaTime);

            // Updating world light direction if this is the main light
            if (mainLight) engine.currentWorld.lightDirection = forwardVector;
        }
    }
}
