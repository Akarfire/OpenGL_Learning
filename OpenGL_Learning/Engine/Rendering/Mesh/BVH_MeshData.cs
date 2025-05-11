using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Rendering.Mesh
{
    public class RayTracingMeshData: MeshData
    {
        public List<RenderTriangle> BVH_triangles { get; protected set; } = new List<RenderTriangle>();


        public RayTracingMeshData(List<Vertex> inVertices = null, List<Triangle> inTriangles = null, NormalCalculationParams inNormalCalculationParams = default(NormalCalculationParams)):
            base(inVertices, inTriangles, inNormalCalculationParams)
        { }


        // Called by the RayTracingRenderEngine on engine start up
        public void GenerateBVH()
        { 
            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle triangle = triangles[i];
                RenderTriangle newTriangle = new RenderTriangle();

                newTriangle.v1 = vertices[(int)triangle.v1].position;
                newTriangle.v2 = vertices[(int)triangle.v2].position;
                newTriangle.v3 = vertices[(int)triangle.v3].position;

                newTriangle.normalX = triangle.normal.X;
                newTriangle.normalY = triangle.normal.Y;
                newTriangle.normalZ = triangle.normal.Z;

                BVH_triangles.Add(newTriangle);
            }
        }
    }
}
