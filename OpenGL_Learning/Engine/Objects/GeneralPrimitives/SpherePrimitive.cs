using OpenGL_Learning.Engine.Rendering;

namespace OpenGL_Learning.Engine.Objects.GeneralPrimitives
{
    public class SpherePrimitive: PrimitiveObject
    {
        // Radius of the primitive sphere
        public float Radius { get; set; } = 1;

        public SpherePrimitive(Engine inEngine, float radius = 1) : base(inEngine)
        {
            Radius = radius;
        }

        // Returns general primitive data structure, that is to be sent to GPU's SSBO
        public override GeneralPrimitive GetPrimitiveData()
        { 
            GeneralPrimitive primitiveData = base.GetPrimitiveData();

            // MaterialID is filled automatically by the base class implementation

            primitiveData.type = 0;
            primitiveData.scalar = Radius * scale.X;
            primitiveData.vecOne = location;

            return primitiveData;
        }
    }
}
