using OpenGL_Learning.Engine.Scripts;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenTK.Mathematics;

namespace GameCode
{
    internal class WaterBouancyScript : Script
    {
        PhysicsScript physicsScript = null;

        // Parameters
        public string ownerPhysicsScriptName;
        public float forceMultiplier = 1.0f;
        public Vector3 originOffset = Vector3.Zero;

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
                Vector3 origin = physicsScript.ownerWO.location - originOffset;
                float time = owner.engine.currentWorld.time * 0.5f;

                // sin(aPosition.x / 4 + time * 0.5)
                //float surfaceLevel = (float)Math.Sin(physicsScript.ownerWO.location.X / 4 + owner.engine.currentWorld.time * 0.5);
                float surfaceLevel = GetWaterHeight(origin, time);

                if (origin.Y < surfaceLevel)
                {
                    physicsScript.AddForce(
                        forceMultiplier * -1 * physicsScript.gravityDirection * physicsScript.gravityAcceleration * deltaTime * physicsScript.objectMass
                        * 2 * (float)Math.Clamp((origin.Y - surfaceLevel) * (origin.Y - surfaceLevel), 0f, 1)

                        + (Vector3.UnitX + Vector3.UnitY).Normalized() * -0.000000025f
                        );


                    // Rotation calculation
                    float finiteOffset = 0.1f;

                    float dx = (GetWaterHeight(origin, time) - GetWaterHeight(origin + Vector3.UnitX * finiteOffset, time)) / finiteOffset;
                    float dz = (GetWaterHeight(origin, time) - GetWaterHeight(origin + Vector3.UnitZ * finiteOffset, time)) / finiteOffset;

                    // 1. Create surface normal
                    Vector3 surfaceNormal = new Vector3(-dx, 1.0f, -dz).Normalized();

                    // 2. Calculate pitch and roll based on normal
                    // Pitch: rotation around X axis (forward/backward tilt)
                    // Roll: rotation around Z axis (sideways tilt)

                    float pitch = MathF.Atan2(surfaceNormal.Z, surfaceNormal.Y);
                    float roll = MathF.Atan2(surfaceNormal.X, surfaceNormal.Y);

                    // Optional: convert from radians to degrees
                    pitch = MathHelper.RadiansToDegrees(pitch);
                    roll = MathHelper.RadiansToDegrees(roll);

                    Vector3 currentRotation = physicsScript.ownerWO.rotation;
                    Vector3 newRotation = new Vector3(
                        (roll - currentRotation.X) * 10 * deltaTime, 
                        0, 
                        (pitch - currentRotation.Z) * 10 * deltaTime);
                    physicsScript.ownerWO.AddRotation(newRotation);
                }
            }

            base.OnScriptUpdated(deltaTime);
        }

        public static float GetWaterHeight(Vector3 worldPos, float time)
        {
            // Wave 1
            float wave1 = MathF.Sin(Vector2.Dot(new Vector2(worldPos.X, worldPos.Z) / 25, new Vector2(0.3f, 0.7f)) * 4.0f + time * 1.2f);

            // Wave 2
            float wave2 = MathF.Sin(Vector2.Dot(new Vector2(worldPos.X, worldPos.Z) / 25, new Vector2(0.8f, -0.6f)) * 6.0f + time * 1.5f);

            // Wave 3
            float wave3 = MathF.Sin(Vector2.Dot(new Vector2(worldPos.X, worldPos.Z) / 25, new Vector2(-0.5f, 0.5f)) * 10.0f + time * 2.0f);

            // Combine waves
            float height = (wave1 + wave2 * 0.5f + wave3 * 0.25f) * 0.5f;

            return 2 * height;
        }
    }
}
