using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Objects
{
    public class RayTracingMeshObject: MeshObject
    {
        public int materialID { get; set; } = 0;

        public RayTracingMeshObject(Engine inEngine, string meshDataName) : base(inEngine, meshDataName)
        {

        }

        // Called by the RayTracingRenderingEngine, this data will be sent to the GPU
        // BVH data is filled automatically in the Rendering Engine
        public ObjectData GetRayTracingObjectData()
        {
            ObjectData objectData = new ObjectData();

            objectData.materialID = materialID;

            return objectData;
        }
    }
}
