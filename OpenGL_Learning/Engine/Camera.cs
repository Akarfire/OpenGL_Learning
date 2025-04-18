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
    public class Camera: GameWorldObject
    {

        private float speed = 6f;
        private int screenWidth;
        private int screenHeight;
        private float fov = 60f;
        private float sensitivity = 60f;

        Vector3 up = Vector3.UnitY;
        Vector3 forward = -Vector3.UnitZ;
        Vector3 right = Vector3.UnitX;

        private bool firstMove = true;
        public Vector2 lastMousePosition;

        bool enableMouseInput = true;


        // -----

        public Camera(Engine inEngine): base(inEngine)
        {
            screenHeight = engine.windowHeight;
            screenWidth = engine.windowHeight;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(location, location + forward, up);
        }
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)screenWidth / screenHeight, 0.1f, 100f);
        }

        public void InputController(KeyboardState keyboardInput, MouseState mouseInput, FrameEventArgs eventArgs)
        {
            float deltaTime = (float)eventArgs.Time;

            if (keyboardInput.IsKeyDown(Keys.W)) { AddLocation(forwardVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.S)) { AddLocation(forwardVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.A)) { AddLocation(rightVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.D)) { AddLocation(rightVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.Q)) { AddLocation(upVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.E)) { AddLocation(upVector * speed * deltaTime); }

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

                AddRotation(new Vector3(0, deltaY * sensitivity * deltaTime, deltaX * sensitivity * deltaTime));
            }
        }

        public void Update(KeyboardState keyboardInput, MouseState mouseInput, FrameEventArgs eventArgs)
        {
            InputController(keyboardInput, mouseInput, eventArgs);
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
