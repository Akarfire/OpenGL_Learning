using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenGL_Learning.Engine.Player;


namespace OpenGL_Learning.Engine.Objects.Player
{
    public class Camera : GameWorldObject, InputInterface
    {

        public float speed = 6f;
        public float fov = 60f;
        public float sensitivity = 30f;
        public float minViewDistance = 0.1f;
        public float maxViewDistance = 200f;

        private int screenWidth;
        private int screenHeight;

        private bool firstMove = true;
        public Vector2 lastMousePosition;

        bool enableMouseInput = true;
        public bool enableInput = true;
        public bool forceFreeCam = false;

        // -----

        public Camera(Engine inEngine) : base(inEngine)
        {
            screenHeight = engine.windowHeight;
            screenWidth = engine.windowHeight;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(location, location + forwardVector, upVector);
        }
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)screenWidth / screenHeight, minViewDistance, maxViewDistance);
        }

        public void InputController(float deltaTime, KeyboardState keyboardInput, MouseState mouseInput)
        {
            Console.Clear();
            Console.WriteLine(forwardVector);

            // Force free cam trigger
            if (keyboardInput.IsKeyDown(Keys.F5)) forceFreeCam = true;
            else if (keyboardInput.IsKeyDown(Keys.F6)) forceFreeCam = false;

            // Main input
            if (!enableInput && !forceFreeCam) return;

            if (keyboardInput.IsKeyDown(Keys.W)) { AddLocation(forwardVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.S)) { AddLocation(-1 * forwardVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.A)) { AddLocation(-1 * rightVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.D)) { AddLocation(rightVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.Q)) { AddLocation(-1 * upVector * speed * deltaTime); }
            if (keyboardInput.IsKeyDown(Keys.E)) { AddLocation(upVector * speed * deltaTime); }

            
            if (firstMove)
            {
                lastMousePosition = new Vector2(mouseInput.X, mouseInput.Y);
                firstMove = false;
            }

            else if (enableMouseInput || forceFreeCam)
            {
                var deltaX = mouseInput.X - lastMousePosition.X;
                var deltaY = mouseInput.Y - lastMousePosition.Y;

                lastMousePosition.X = mouseInput.X;
                lastMousePosition.Y = mouseInput.Y;


                AddRotation(new Vector3(0, deltaX * sensitivity * deltaTime, -1 * deltaY * sensitivity * deltaTime));

                // Clamping pitch
                Vector3 clampedRotation = rotation;
                clampedRotation.Z = Math.Clamp(rotation.Z, -89, 89);
                rotation = clampedRotation;

            }
        }

        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
        {
            InputController(deltaTime, keyboardState, mouseState);
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
