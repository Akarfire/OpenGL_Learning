using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine
{
    public class GameWorldObject: GameObject
    {
        // Location of the object on the previous frame
        private Vector3 previousFrameLocation;

        // Object's location in world space
        public Vector3 location { get; protected set; }
        // Object's rotation in world space
        public Quaternion rotation { get; protected set; }
        // Object's scale in world space
        public Vector3 scale { get; protected set; }
        // Object's velocity in world space
        public Vector3 velocity { get; protected set; }


        // Directional vectors

        public Vector3 forwardVector { get; protected set; }
        public Vector3 upVector { get; protected set; }
        public Vector3 rightVector { get; protected set; }


        public GameWorldObject(Engine inEngine) : base(inEngine) { }

        public override void onSpawned()
        {
            base.onSpawned();
            previousFrameLocation = location;
        }
        public override void onUpdated(float deltaTime)
        {
            base.onUpdated(deltaTime);

            // Calculating velocity
            velocity = location - previousFrameLocation;
            previousFrameLocation = location;
        }


        // Sets a new location for this object in world space
        public void SetLocation(Vector3 location) { this.location = location; OnTransformationUpdated(); }

        // Adds world-space offset to the object's location
        public void AddLocation(Vector3 location) { this.location += location; OnTransformationUpdated(); }


        // Sets a new rotation for this object in world space in form a quaternion
        public void SetRotation(Quaternion rotation) { this.rotation = rotation; OnTransformationUpdated(); }

        // Sets a new rotation for this object in world space in form a vector of angles
        public void SetRotation(Vector3 angles) { this.rotation = Quaternion.FromEulerAngles(angles); OnTransformationUpdated(); }

        // Adds angular offset to the object in world space
        public void AddRotation(Quaternion rotation) { this.rotation *= rotation; OnTransformationUpdated(); }

        // Adds angular offset to the object in world space
        public void AddRotation(Vector3 angles) { this.rotation *= Quaternion.FromEulerAngles(angles); OnTransformationUpdated(); }


        // Sets object's scale
        public void SetScale(Vector3 scale) { this.scale = scale; OnTransformationUpdated(); }


        // Called whenether object's location, rotation or scale have been changed
        protected virtual void OnTransformationUpdated() 
        {
            // Recalculating directional vectors
            forwardVector = rotation * Vector3.UnitX;
            rightVector = Vector3.Normalize(Vector3.Cross(forwardVector, Vector3.UnitY));
            upVector = Vector3.Normalize(Vector3.Cross(rightVector, forwardVector));
        }
    }
}
