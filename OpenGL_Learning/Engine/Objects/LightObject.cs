using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenGL_Learning.Engine.Objects
{
    public class LightObject: GameWorldObject
    {

        // Light data of this light object
        protected LightData lightData;

        public LightObject(Engine inEngine): base(inEngine) {}

        protected override void OnTransformationUpdated()
        {
            base.OnTransformationUpdated();

            lightData.location = location;
            lightData.direction = forwardVector;
        }

        public ref LightData GetLightData() { return ref lightData; }


        // Chaning parameters
        public void SetLightColor(Color color) 
        {
            lightData.lightColor = new Vector3(
                (float)color.R / 255,
                (float)color.G / 255,
                (float)color.B / 255);
        }

        public void SetLightColor(Vector3 color)
        {
            lightData.lightColor = color;
        }


        public void SetLightIntensity(float intensity) { lightData.intensity = intensity; }
    }
}
