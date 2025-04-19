using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine.objects;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.objects.meshObjects
{
    internal class CubeObject : MeshObject
    {
        public CubeObject(Engine inEngine, string shaderHandle, string[] textureHandles) : base(inEngine, shaderHandle, textureHandles)
        {

            vertices = new List<Vertex>()
            {
                new Vertex(new Vector3( -0.5f,  0.5f,   0.5f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f,  0.5f,   0.5f), new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f, -0.5f,   0.5f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector3( -0.5f, -0.5f,   0.5f), new Vector2(0.0f, 1.0f)),

                new Vertex(new Vector3( -0.5f,  0.5f,   -0.5f), new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f,  0.5f,   -0.5f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector3(  0.5f, -0.5f,   -0.5f), new Vector2(0.0f, 1.0f)),
                new Vertex(new Vector3( -0.5f, -0.5f,   -0.5f), new Vector2(1.0f, 1.0f))
            };

            triangles = new List<Triangle>()
            {
                new Triangle(0, 1, 2),
                new Triangle(2, 3, 0),

                new Triangle(4, 5, 6),
                new Triangle(6, 7, 4),

                new Triangle(1, 5, 6),
                new Triangle(6, 2, 1),

                new Triangle(0, 4, 7),
                new Triangle(7, 3, 0),

                new Triangle(0, 4, 5),
                new Triangle(5, 1, 0),

                new Triangle(3, 2, 6),
                new Triangle(6, 7, 3)
            };

            InitMeshObject();
        }
    }
}
