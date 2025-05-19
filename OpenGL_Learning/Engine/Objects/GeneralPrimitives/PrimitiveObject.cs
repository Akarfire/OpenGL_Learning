using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.RenderEngines;

namespace OpenGL_Learning.Engine.Objects.GeneralPrimitives
{
    public abstract class PrimitiveObject: GameWorldObject
    {
        public int materialID { get; protected set; } = 0;


        public PrimitiveObject(Engine inEngine) : base(inEngine) {}


        // Returns general primitive data structure, that is to be sent to GPU's SSBO
        public virtual GeneralPrimitive GetPrimitiveData()
        {
            GeneralPrimitive primitiveData = new GeneralPrimitive();

            primitiveData.materialID = materialID;

            return primitiveData;
        }


        // Sets a new material for the object's geometry
        public void SetMaterial(string materialName)
        {
            RayTracingRenderEngine renderEngine;
            if (engine.renderingEngine is RayTracingRenderEngine)
            {
                renderEngine = (RayTracingRenderEngine)engine.renderingEngine;
                materialID = renderEngine.materials.Keys.ToList().IndexOf(materialName);

                // If material was not found, we set it to default material
                if (materialID < 0) materialID = 0;
            }
        }
    }
}
