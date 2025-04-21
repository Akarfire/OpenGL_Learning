using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine.objects;
using OpenTK.Mathematics;
using Assimp;

namespace OpenGL_Learning.Engine.Objects.MeshObjects
{
    internal class FbxMeshObject: MeshObject
    {
        public FbxMeshObject(Engine inEngine, string fbxFilePath, string shaderHandle = null, string[] textureHandles = null) : base(inEngine, shaderHandle, textureHandles)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(fbxFilePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

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


            InitMeshObject();
        }
    }
}
