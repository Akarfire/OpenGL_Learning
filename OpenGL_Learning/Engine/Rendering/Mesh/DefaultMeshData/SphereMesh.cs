using OpenGL_Learning.Engine.Rendering.Mesh;
using OpenTK.Mathematics;
using System.Security.Principal;

namespace OpenGL_Learning.Engine.Rendering.DefaultMeshData
{
    public class SphereMesh: MeshData
    {
        public SphereMesh(int sectorCount = 36, int stackCount = 18, float radius = 0.5f) 
        {

            vertices = new List<Vertex>();
            triangles = new List<Triangle>();

            // Generate vertices
            for (int i = 0; i <= stackCount; ++i)
            {
                float stackAngle = MathF.PI / 2 - i * MathF.PI / stackCount; // from pi/2 to -pi/2
                float xy = radius * MathF.Cos(stackAngle); // r * cos(u)
                float z = radius * MathF.Sin(stackAngle);  // r * sin(u)

                for (int j = 0; j <= sectorCount; ++j)
                {
                    float sectorAngle = j * 2 * MathF.PI / sectorCount; // from 0 to 2pi

                    float x = xy * MathF.Cos(sectorAngle); // r * cos(u) * cos(v)
                    float y = xy * MathF.Sin(sectorAngle); // r * cos(u) * sin(v)

                    float u = (float)j / sectorCount;
                    float v = (float)i / stackCount;

                    Vector3 position = new Vector3(x, y, z);
                    Vector3 normal = (position).Normalized();

                    vertices.Add(new Vertex(new Vector3(x, y, z), normal, new Vector2(u, v)));
                }
            }

            // Generate triangle indices
            for (int i = 0; i < stackCount; ++i)
            {
                int k1 = i * (sectorCount + 1); // beginning of current stack
                int k2 = k1 + sectorCount + 1;  // beginning of next stack

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        triangles.Add(new Triangle((uint)k1, (uint)k2, (uint)k1 + 1));
                    }
                    if (i != (stackCount - 1))
                    {
                        triangles.Add(new Triangle((uint)k1 + 1, (uint)k2, (uint)k2 + 1));
                    }
                }
            }

            normalCalculationParams.enableTriangleNormals = false;
            InitMeshData();

            InitMeshData();
        }
    }
}
