using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

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

        public Triangle(uint inV1, uint inV2, uint inV3) { v1 = inV1; v2 = inV2; v3 = inV3; }
    }


    public class MeshData
    {
        // Mesh data lists
        protected List<Vertex> vertices = new List<Vertex>();
        protected List<Triangle> triangles = new List<Triangle>();

        // Generated Mesh data
        protected Vector3[] genVertices { get; private set; }
        protected uint[] genIndices { get; private set; }
        protected Vector2[] genTexCoords { get; private set; }

        // Buffer handlers
        int VBO;
        int VAO;
        int EBO;
        int textureVBO;

        public MeshData(List<Vertex> inVertices = null, List<Triangle> inTriangles = null) 
        { 
            vertices = inVertices; triangles = inTriangles;

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


        // Converts vertex and triangle lists into data, that will be used for rendering
        private void generateMeshData()
        {
            genVertices = new Vector3[vertices.Count];
            genTexCoords = new Vector2[vertices.Count];
            genIndices = new uint[triangles.Count * 3];

            for (int i = 0; i < vertices.Count; i++)
            {
                genVertices[i] = vertices[i].position;
                genTexCoords[i] = vertices[i].texCoords;
            }

            int index = 0;
            foreach (var triangle in triangles)
            {
                genIndices[index] = triangle.v1;
                genIndices[index + 1] = triangle.v2;
                genIndices[index + 2] = triangle.v3;

                index += 3;
            }
        }


        // Must be called at the end of child constructor
        protected void InitMeshData()
        {
            //
            generateMeshData();

            // Generating GL buffers
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            EBO = GL.GenBuffer();
            textureVBO = GL.GenBuffer();

            // Vertex array
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, genVertices.Length * Vector3.SizeInBytes, genVertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);

            // EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, genIndices.Length * sizeof(uint), genIndices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // texture VBO
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, genTexCoords.Length * Vector2.SizeInBytes, genTexCoords, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);

            // Bindinng cleanup
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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
            GL.DrawElements(PrimitiveType.Triangles, genIndices.Length * sizeof(uint), DrawElementsType.UnsignedInt, 0);
        }
    }
}
