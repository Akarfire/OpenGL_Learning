using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Scripts;
using OpenTK.Mathematics;

namespace OpenGL_Learning.RayTracingTest
{
    public class LightColorChangerScript: Script
    {
        LightObject ownerLight = null;

        //------

        public LightColorChangerScript() { }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is LightObject) ownerLight = (LightObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerLight != null) 
            {
                float time = owner.engine.currentWorld.time;

                ownerLight.SetLightColor(new Vector3((MathF.Sin(time) + 1) / 2, (MathF.Sin(time + 3.14f) + 1) / 2, 1));
            }
        }
    }
}
