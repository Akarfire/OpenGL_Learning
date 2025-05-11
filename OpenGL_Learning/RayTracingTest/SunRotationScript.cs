using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Scripts;
using OpenTK.Mathematics;

namespace OpenGL_Learning.RayTracingTest
{
    public class SunRotationScript: Script
    {
        GameWorldObject ownerWO = null;
        public SunRotationScript() { }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is GameWorldObject) ownerWO = (GameWorldObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerWO != null) 
            {
                ownerWO.AddRotation(new Vector3(0, 1f * deltaTime, 0.025f * deltaTime));
            }
        }
    }
}
