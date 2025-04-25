using OpenGL_Learning.Engine.Objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Scripts.EngineScripts
{
    internal class PhysicsScript: Script
    {

        public GameWorldObject ownerWO { get; private set; } = null;

        public Vector3 totalForce {  get; protected set; }

        // Parameters
        public float objectMass = 1f;


        // Default forces parameters

            // Drug force parameters
            public bool enableDragForce = true;
            public float dragForceStrenght = 0.25f;

            // Gravity force parameters
            public bool enableGravityForce = false;
            public Vector3 gravityDirection = Vector3.UnitY * -1;
            public float gravityAcceleration = 0.0098f;

        //------

        public PhysicsScript() { }


        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is GameWorldObject) ownerWO = (GameWorldObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerWO != null) 
            {
                // Default forces
                if (enableDragForce) DragForce(deltaTime);
                if (enableGravityForce) GravityForce(deltaTime);

                // Forces application
                ownerWO.AddLocation(totalForce);
            }
        }


        // Adds external force to the object
        public void AddForce(Vector3 force) { totalForce += force; }


        // Default forces
        protected void DragForce(float deltaTime)
        {
            AddForce(totalForce * -1 * dragForceStrenght * deltaTime);
        }

        protected void GravityForce(float deltaTime) 
        {
            AddForce(gravityDirection * objectMass * gravityAcceleration * deltaTime);
        }
    }
}
