using OpenGL_Learning.Engine.Objects.Player;
using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Scripts;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameCode
{
    public class FollowCamera: Script
    {
        GameWorldObject ownerGW = null;

        // Mouse input
        protected Vector2 lastMousePosition;
        protected bool firstMove = true;

        // Parameters
        public float speed = 3f;
        public float distance = 15.0f;

        protected float yaw = 0.0f;
        protected float pitch = 0.0f;

        public float sensitivity = 15f;

        public FollowCamera() { }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is GameWorldObject) ownerGW = (GameWorldObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerGW != null)
            {
                Camera camera = owner.engine.currentWorld.worldCamera;

                // Do nothing if freeCam is forced
                if (camera.forceFreeCam) return;

                camera.enableInput = false;

                Vector3 targetLocation = ownerGW.location + Matrix3.CreateRotationY(MathHelper.DegreesToRadians(yaw)) * Matrix3.CreateRotationZ(MathHelper.DegreesToRadians(pitch)) * Vector3.UnitX * -1 * distance;

                Vector3 lookDirection = (ownerGW.location - camera.location).Normalized(); 
                Vector3 targetRotation = new Vector3(0, MathHelper.RadiansToDegrees(MathF.Atan2(lookDirection.Z, lookDirection.X)), MathHelper.RadiansToDegrees(MathF.Asin(lookDirection.Y)));

                // Interpolation
                camera.AddLocation((targetLocation - camera.location) * speed * deltaTime);   
                camera.SetRotation(targetRotation);
            }
        }

        public void MouseInput(float deltaTime, MouseState mouseState)
        {
            if (firstMove)
            {
                lastMousePosition = mouseState.Position;
                firstMove = false;
            }

            var deltaX = mouseState.X - lastMousePosition.X;
            var deltaY = mouseState.Y - lastMousePosition.Y;

            lastMousePosition.X = mouseState.X;
            lastMousePosition.Y = mouseState.Y;

            yaw += deltaX * sensitivity * deltaTime;
            pitch += deltaY * sensitivity * deltaTime;

            pitch = Math.Clamp(pitch, 10, 89);

        }
    }
}
