using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.Rendering.DefaultMeshData
{
    public class RenderPlaneMesh: MeshData
    {
        public RenderPlaneMesh()
        {
            vertices = new List<Vertex>
            {
                new Vertex(new Vector3( -1f,  1f,   -1f), new Vector2(0.0f, 1.0f)),
                new Vertex(new Vector3(  1f,  1f,   -1f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector3(  1f, -1f,   -1f), new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector3( -1f, -1f,   -1f), new Vector2(0.0f, 0.0f))
            };

            triangles = new List<Triangle>
            {
                new Triangle(0, 1, 2),
                new Triangle(2, 3, 0)
            };

            InitMeshData();
        }
    }
}
