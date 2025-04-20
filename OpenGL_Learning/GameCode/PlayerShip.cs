using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.objects.meshObjects;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameCode
{
    internal class PlayerShip : CubeObject, InputInterface
    {

        // Scripts
        PhysicsScript physicsScript = null;
        WaterBouancyScript bouancyScript = null;

        // Parameters
        public float speed = 0.25f;
        public float force = 0.01f;
        public float rotationSpeed = 40f;

        // ----

        public PlayerShip(Engine inEngine) : base(inEngine)
        {
            SetShader("Default_S");
            SetTextures(new string[] { "Wood_T" });

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

            SetScale(new Vector3(5, 5, 1));
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
