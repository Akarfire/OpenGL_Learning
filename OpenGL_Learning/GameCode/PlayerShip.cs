using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Scripts;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenGL_Learning.Engine.Player;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Objects;

namespace GameCode
{
    internal class PlayerShip : MeshObject, InputInterface
    {

        // Scripts
        PhysicsScript physicsScript = null;
        WaterBouancyScript bouancyScript = null;
        FollowCamera followCameraScript = null;

        // Parameters
        public float speed = 5f;
        public float force = 0.01f;
        public float rotationSpeed = 40f;

        // ----

        public PlayerShip(Engine inEngine) : base(inEngine, "Ship_M")
        {
            SetShader("Default_S");
            SetTextures(new string[] { "Shooter_T" });

            physicsScript = new PhysicsScript();

            physicsScript.enableGravityForce = true;
            physicsScript.enableDragForce = true;
            physicsScript.dragForceStrenght = 0.6f;
            physicsScript.objectMass = 2f;

            AddScript("Physics", physicsScript);
            physicsScript.AttachScript(this);


            bouancyScript = new WaterBouancyScript();

            AddScript("Bouancy", bouancyScript);
            bouancyScript.AttachScript(this);
            bouancyScript.originOffset = Vector3.UnitY * 0.65f;
            bouancyScript.forceMultiplier = 2f;


            followCameraScript = new FollowCamera();
            AddScript("followCamera", followCameraScript);


            SetScale(new Vector3(2, 2, 2));
        }

        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
        {
            if (keyboardState.IsKeyDown(Keys.W)) physicsScript.AddForce(forwardVector * speed * force * deltaTime);
            if (keyboardState.IsKeyDown(Keys.S)) physicsScript.AddForce(-1 * forwardVector * speed * force * deltaTime);
            if (keyboardState.IsKeyDown(Keys.D)) AddRotation(Vector3.UnitY * rotationSpeed * deltaTime);
            if (keyboardState.IsKeyDown(Keys.A)) AddRotation(-1 * Vector3.UnitY * rotationSpeed * deltaTime);

            if (keyboardState.IsKeyDown(Keys.LeftShift)) physicsScript.objectMass -= 1f * deltaTime;
            if (keyboardState.IsKeyDown(Keys.LeftControl)) physicsScript.objectMass += 1f * deltaTime;

            physicsScript.objectMass = Math.Clamp(physicsScript.objectMass, 2f, 8f);

            followCameraScript.MouseInput(deltaTime, mouseState);
        }
    }
}
