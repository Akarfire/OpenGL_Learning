using OpenGL_Learning.Engine.Rendering.Mesh;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.Rendering.DefaultMeshData
{
    internal class PlaneMesh: RayTracingMeshData
    {
        public PlaneMesh() 
        {
            vertices = new List<Vertex>
            {
                new Vertex(new Vector3( -0.5f,  0.5f,   0f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f,  0.5f,   0f), new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f, -0.5f,   0f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector3( -0.5f, -0.5f,   0f), new Vector2(0.0f, 1.0f))
            };

            triangles = new List<Triangle>
            {
                new Triangle(0, 1, 2),
                new Triangle(2, 3, 0)
            };

            normalCalculationParams.enableTriangleNormals = true;

            InitMeshData();
        }
    }
}
