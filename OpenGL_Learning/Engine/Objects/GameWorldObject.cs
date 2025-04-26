using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.Objects
{
    public class GameWorldObject : GameObject
    {
        // Location of the object on the previous frame
        private Vector3 previousFrameLocation;

        // Object's location in world space
        public Vector3 location { get; protected set; } = new Vector3(0, 0, 0);
        // Object's rotation in world space
        public Vector3 rotation { get; protected set; } = new Vector3(0, 0, 0);
        // Object's scale in world space
        public Vector3 scale { get; protected set; } = new Vector3(1, 1, 1);
        // Object's velocity in world space
        public Vector3 velocity { get; protected set; } = new Vector3(0, 0, 0);


        // Directional vectors

        public Vector3 forwardVector { get; protected set; }
        public Vector3 upVector { get; protected set; }
        public Vector3 rightVector { get; protected set; }

        public GameWorldObject(Engine inEngine) : base(inEngine) { OnTransformationUpdated(); }

        public override void OnSpawned()
        {
            base.OnSpawned();
            previousFrameLocation = location;
        }
        public override void OnUpdated(float deltaTime)
        {
            base.OnUpdated(deltaTime);

            // Calculating velocity
            velocity = location - previousFrameLocation;
            previousFrameLocation = location;
        }


        // Sets a new location for this object in world space
        public void SetLocation(Vector3 location, bool teleport = true)
        {
            this.location = location;

            if (teleport) { previousFrameLocation = this.location; }

            OnTransformationUpdated();
        }

        // Adds world-space offset to the object's location
        public void AddLocation(Vector3 location, bool teleport = false)
        {
            this.location += location;

            if (teleport) { previousFrameLocation = this.location; }

            OnTransformationUpdated();
        }


        // Sets a new rotation for this object in world space in form a quaternion
        public void SetRotation(Vector3 rotation) { this.rotation = rotation; OnTransformationUpdated(); }

        // Adds angular offset to the object in world space
        public void AddRotation(Vector3 additiveRotation)
        {
            rotation += additiveRotation;
            OnTransformationUpdated();
        }


        // Sets object's scale
        public void SetScale(Vector3 scale) { this.scale = scale; OnTransformationUpdated(); }


        // Called whenether object's location, rotation or scale have been changed
        protected virtual void OnTransformationUpdated()
        {
            // Recalculating directional vectors

            Vector3 newForwardVector = new Vector3();
            newForwardVector.X = MathF.Cos(MathHelper.DegreesToRadians(rotation.Z)) * MathF.Cos(MathHelper.DegreesToRadians(rotation.Y));
            newForwardVector.Y = MathF.Sin(MathHelper.DegreesToRadians(rotation.Z));
            newForwardVector.Z = MathF.Cos(MathHelper.DegreesToRadians(rotation.Z)) * MathF.Sin(MathHelper.DegreesToRadians(rotation.Y));

            forwardVector = Vector3.Normalize(newForwardVector);

            rightVector = Vector3.Normalize(Vector3.Cross(forwardVector, Vector3.UnitY));
            upVector = Vector3.Normalize(Vector3.Cross(rightVector, forwardVector));
        }
    }
}
