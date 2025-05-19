using Assimp;
using OpenGL_Learning.Engine.Objects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Utilities
{
    static class GeometryUtilities
    {

        // Returns the position of a random vertex, that belongs to this mesh (in world space)
        public static Vector3 GetRandomVertexPosition(MeshObject meshObject) 
        {
            Vector3 position = Vector3.Zero;

            if (meshObject.meshData.vertices.Count > 0)
            {
                int vertexIndex = RandomNumberGenerator.GetInt32(meshObject.meshData.vertices.Count - 1);
                position = meshObject.meshData.vertices[vertexIndex].position;

                position = (new Vector4(position, 1) * meshObject.GetModelMatrix()).Xyz;
            }

            return position;
        }
    }
}
