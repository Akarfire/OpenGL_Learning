using OpenGL_Learning.Engine;
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

        // Parameters
        public float speed = 0.25f;
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
            physicsScript.dragForceStrenght = 0.8f;
            physicsScript.objectMass = 0.25f;

            AddScript("Physics", physicsScript);
            physicsScript.AttachScript(this);


            bouancyScript = new WaterBouancyScript();

            AddScript("Bouancy", bouancyScript);
            bouancyScript.AttachScript(this);

            SetScale(new Vector3(2, 2, 2));
        }

        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
        {
            if (keyboardState.IsKeyDown(Keys.Up)) physicsScript.AddForce(forwardVector * speed * force * deltaTime);
            if (keyboardState.IsKeyDown(Keys.Down)) physicsScript.AddForce(-1 * forwardVector * speed * force * deltaTime);
            if (keyboardState.IsKeyDown(Keys.Right)) AddRotation(Vector3.UnitY * rotationSpeed * deltaTime);
            if (keyboardState.IsKeyDown(Keys.Left)) AddRotation(-1 * Vector3.UnitY * rotationSpeed * deltaTime);
        }
    }
}
