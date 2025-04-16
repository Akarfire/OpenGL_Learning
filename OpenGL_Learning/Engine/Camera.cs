using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OpenGL_Learning.Engine
{
    internal class Camera
    {

        private float speed = 6f;
        private int screenWidth;
        private int screenHeight;
        private float fov = 60f;
        private float sensitivity = 60f;

        public Vector3 position;

        Vector3 up = Vector3.UnitY;
        Vector3 forward = -Vector3.UnitZ;
        Vector3 right = Vector3.UnitX;

        private float pitch;
        private float yaw = -90f;

        private bool firstMove = true;
        public Vector2 lastMousePosition;

        bool enableMouseInput = true;

        public Camera(int width, int height, Vector3 inPosition)
        {
            screenHeight = height;
            screenWidth = width;
            position = inPosition;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + forward, up);
        }
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)screenWidth / screenHeight, 0.1f, 100f);
        }

        public void InputController(KeyboardState keyboardInput, MouseState mouseInput, FrameEventArgs eventArgs)
        {
            float deltaTime = (float)eventArgs.Time;

            if (keyboardInput.IsKeyDown(Keys.W)) { position += forward * speed * deltaTime; }
            if (keyboardInput.IsKeyDown(Keys.S)) { position -= forward * speed * deltaTime; }
            if (keyboardInput.IsKeyDown(Keys.A)) { position -= right * speed * deltaTime; }
            if (keyboardInput.IsKeyDown(Keys.D)) { position += right * speed * deltaTime; }
            if (keyboardInput.IsKeyDown(Keys.Q)) { position -= up * speed * deltaTime; }
            if (keyboardInput.IsKeyDown(Keys.E)) { position += up * speed * deltaTime; }

            if (firstMove)
            {
                lastMousePosition = new Vector2(mouseInput.X, mouseInput.Y);
                firstMove = false;
            }

            else if (enableMouseInput)
            {
                var deltaX = mouseInput.X - lastMousePosition.X;
                var deltaY = mouseInput.Y - lastMousePosition.Y;

                lastMousePosition.X = mouseInput.X;
                lastMousePosition.Y = mouseInput.Y;

                yaw += deltaX * sensitivity * deltaTime;
                pitch -= deltaY * sensitivity * deltaTime;

                UpdateVectors();
            }
        }

        public void Update(KeyboardState keyboardInput, MouseState mouseInput, FrameEventArgs eventArgs)
        {
            InputController(keyboardInput, mouseInput, eventArgs);
        }


        private void UpdateVectors()
        {
            forward.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
            forward.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            forward.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));

            forward = Vector3.Normalize(forward);

            right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

            up = Vector3.Normalize(Vector3.Cross(right, forward));
        }

        public void SetMouseInputEnabled(bool enabled)
        {
            enableMouseInput = enabled;

            if (enableMouseInput) { firstMove = true; }
        }


        public void UpdateWindowSize(int newScreenWidth, int newScreenHeight)
        {
            screenHeight = newScreenHeight;
            screenWidth = newScreenWidth;
        }
    }
}
