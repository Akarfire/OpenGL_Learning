using OpenGL_Learning.Engine.Objects;
using OpenTK.Mathematics;
using Assimp;
using OpenGL_Learning.Engine.Rendering.Mesh;

namespace OpenGL_Learning.Engine.Rendering.DefaultMeshData
{
    internal class MeshFromFile: MeshData
    {
        public MeshFromFile(string filePath)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            vertices = new List<Vertex>();
            triangles = new List<Triangle>();

            foreach (var mesh in scene.Meshes)
            {
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    var v = mesh.Vertices[i];
                    var n = mesh.Normals[i];
                    var uv = mesh.TextureCoordinateChannels[0][i];

                    vertices.Add(new Vertex(
                        new Vector3(v.X, v.Y, v.Z), new Vector3(n.X, n.Y, n.Z), new Vector2(uv.X, uv.Y)
                        )); ;
                }

                foreach (var tri in mesh.Faces)
                {
                    triangles.Add(new Triangle((uint)tri.Indices[0], (uint)tri.Indices[1], (uint)tri.Indices[2]));
                }
            }

            InitMeshData();
        }
    }
}
