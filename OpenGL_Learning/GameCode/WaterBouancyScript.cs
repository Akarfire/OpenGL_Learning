using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenTK.Mathematics;

namespace GameCode
{
    internal class WaterBouancyScript : Script
    {
        PhysicsScript physicsScript = null;

        // Parameters
        public string ownerPhysicsScriptName;


        // ------

        public WaterBouancyScript(string ownerPhysicsScriptName = "Physics") { this.ownerPhysicsScriptName = ownerPhysicsScriptName; }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner.scripts.ContainsKey(ownerPhysicsScriptName))
            {
                if (owner.scripts[ownerPhysicsScriptName] is PhysicsScript)
                    physicsScript = (PhysicsScript)owner.scripts[ownerPhysicsScriptName];
            }
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (physicsScript != null) 
            {
                float surfaceLevel = 0;

                if (physicsScript.ownerWO.location.Y < surfaceLevel) 
                    physicsScript.AddForce(
                        -1 * physicsScript.gravityDirection * physicsScript.gravityAcceleration * deltaTime * physicsScript.objectMass 
                        * 3 * (float)Math.Clamp(Math.Pow(physicsScript.ownerWO.location.Y - surfaceLevel, 2) / 0.15f, 0.15f, 1)
                        );
            }
        }
    }
}
