﻿using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.RenderEngines;
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
        public int materialID { get; protected set; } = 0;

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
