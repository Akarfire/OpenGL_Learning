using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace OpenGL_Learning.Engine.Rendering
{
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoords;

        public Vertex() { }
        public Vertex(Vector3 inPosition, Vector3 inNormal, Vector2 inTexCoords) { position = inPosition; normal = inNormal; texCoords = inTexCoords; }
        public Vertex(Vector3 inPosition, Vector2 inTexCoords) { position = inPosition; texCoords = inTexCoords; }
    }

    public struct Triangle
    {
        public uint v1, v2, v3;
        public Vector3 normal = Vector3.Zero;

        public Triangle(uint inV1, uint inV2, uint inV3) { v1 = inV1; v2 = inV2; v3 = inV3;}
        public Triangle(uint inV1, uint inV2, uint inV3, Vector3 inNormal) { v1 = inV1; v2 = inV2; v3 = inV3; normal = inNormal; }

        public static Vector3 CalculateTriangleNormal(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 insidePosition)
        {
            Vector3 normal = new Vector3();

            normal = Vector3.Cross(v2 - v1, v3 - v1).Normalized();
            if (Vector3.Dot(normal, ((v1 + v2 + v3) / 3) - insidePosition) < 0) normal *= -1;

            return normal;
        }
    }


    public class MeshData
    {
        // Mesh data lists
        protected List<Vertex> vertices = new List<Vertex>();
        protected List<Triangle> triangles = new List<Triangle>();

        protected bool enableTriangleNormals = false;

        // An inner list of vertices (either references to vertices or contains a list of duplicated vertices (if enableTriangleNormals is set true))
        List<Vertex> genVertices = new List<Vertex>();

        // Buffer handlers
        int VBO;
        int VAO;
        int EBO;
        int textureVBO;

        public MeshData(List<Vertex> inVertices = null, List<Triangle> inTriangles = null, bool inEnableTriangleNormals = false) 
        { 
            vertices = inVertices; triangles = inTriangles; enableTriangleNormals = inEnableTriangleNormals;

            if (vertices != null && triangles != null)
                InitMeshData();
        }

        public void Destroy()
        {
            // Deleting buffers
            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
        }

        // Must be called at the end of child constructor
        protected void InitMeshData()
        {
            // Generating a new list of vertices with duplicated values
            if (enableTriangleNormals)
            {
                // Allows for hard edges (per triangle normals) USES VERTEX DUPLICATION

                for (int i = 0; i < triangles.Count; i++)
                {
                    Vertex v1 = vertices[(int)triangles[i].v1];
                    Vertex v2 = vertices[(int)triangles[i].v2];
                    Vertex v3 = vertices[(int)triangles[i].v3];

                    // Calculating triangle normal if none is specified
                    if (triangles[i].normal == Vector3.Zero)
                    {
                        Triangle tri = triangles[i];
                        tri.normal = Triangle.CalculateTriangleNormal(v1.position, v2.position, v3.position, Vector3.Zero);
                        triangles[i] = tri;
                    }

                    // Updating vertex normals based on triangle normals
                    v1.normal = v2.normal = v3.normal = triangles[i].normal;

                    // Adding vertex duplicates to the new list
                    genVertices.Add(v1);
                    genVertices.Add(v2);
                    genVertices.Add(v3);
                }
            }

            // If no triangle normals are required, we can just use the initial list of vertices
            else genVertices = vertices;


            // Creating VAO and all of it's attributes

            // Vertex array
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);


            // Vertex Buffer array
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, genVertices.Count * Marshal.SizeOf<Vertex>(), genVertices.ToArray(), BufferUsageHint.StaticDraw);

            // Attrib 0: position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                Marshal.SizeOf<Vertex>(),
                Marshal.OffsetOf<Vertex>("position"));
            GL.EnableVertexAttribArray(0);

            // Attrib 1: texture coordinates
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,
                Marshal.SizeOf<Vertex>(),
                Marshal.OffsetOf<Vertex>("texCoords"));
            GL.EnableVertexAttribArray(1);

            // Attrib 2: normals
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false,
                Marshal.SizeOf<Vertex>(),
                Marshal.OffsetOf<Vertex>("normal"));
            GL.EnableVertexAttribArray(2);


            // We only need EBO when we are reusing vertices, if Triangle Normals are enabled, we create copies of vertices for each triangle, so we don't need a EBO
            if (!enableTriangleNormals)
            {
                // Smooth shading (No vertex duplication)

                // Converts list of triangles into an array of indices
                // (to make GPU happy - she does not like structs, she likes her indices raw)
                uint[] genIndices = new uint[triangles.Count * 3];

                for (int i = 0; i < triangles.Count; i++)
                {
                    genIndices[i * 3 + 0] = triangles[i].v1;
                    genIndices[i * 3 + 1] = triangles[i].v2;
                    genIndices[i * 3 + 2] = triangles[i].v3;
                }

                // EBO
                EBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, genIndices.Length * sizeof(uint), genIndices, BufferUsageHint.StaticDraw);
            }

            // Bindinng cleanup
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }


        // Reinitializes mesh data, applying changes to vertices and triangles
        public void ReinitMeshData()
        {
            // Deleting buffers
            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);

            InitMeshData();
        }


        public void Render()
        {
            // Binding vertex array and elements
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            // Draw call
            if (enableTriangleNormals) GL.DrawArrays(PrimitiveType.Triangles, 0, genVertices.Count);
            else GL.DrawElements(PrimitiveType.Triangles, triangles.Count * Marshal.SizeOf<Triangle>(), DrawElementsType.UnsignedInt, 0);
        }
    }
}
