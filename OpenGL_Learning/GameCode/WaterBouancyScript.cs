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
            if (physicsScript != null) 
            {
                // sin(aPosition.x / 4 + time * 0.5)
                float surfaceLevel = (float)Math.Sin(physicsScript.ownerWO.location.X / 4 + owner.engine.currentWorld.time * 0.5);

                if (physicsScript.ownerWO.location.Y < surfaceLevel) 
                    physicsScript.AddForce(
                        -1 * physicsScript.gravityDirection * physicsScript.gravityAcceleration * deltaTime * physicsScript.objectMass 
                        * 2 * (float)Math.Clamp((physicsScript.ownerWO.location.Y - surfaceLevel) * (physicsScript.ownerWO.location.Y - surfaceLevel), 0f, 1)
                        
                        + Vector3.UnitX * -0.000000025f
                        );
            }

            base.OnScriptUpdated(deltaTime);
        }
    }
}
