using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Mathematics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Objects.Lights
{
    internal class PointLightObject: LightObject
    {
        public PointLightObject(Engine inEngine, float intensity, Color color, float softness) : base(inEngine) 
        {
            lightData.intensity = intensity;
            lightData.type = 0;
            lightData.softness = softness;

            SetLightColor(color);

            lightData.lightColor = new Vector3(1f, 1f, 1f);
        }
    }
}
