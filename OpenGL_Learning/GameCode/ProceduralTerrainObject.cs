using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine;


namespace GameCode
{
    internal class ProceduralTerrainObject : MeshObject
    {
        protected int sizeX, sizeZ;
        protected float cellSize;
        protected float noiseScale;
        public ProceduralTerrainObject(int inSizeX, int inSizeZ, float inCellSize, float inNoiseScale, float heightM, Engine inEngine, string shaderHandle = null, string[] textureHandles = null) : base(inEngine, null, shaderHandle, textureHandles)
        {
            sizeX = inSizeX;
            sizeZ = inSizeZ;
            cellSize = inCellSize;
            noiseScale = inNoiseScale;

            List<Vertex> vertices = new List<Vertex>();
            List<Triangle> triangles = new List<Triangle>();

            for (int x = 0; x < inSizeX; x++)
                for (int z = 0; z < inSizeZ; z++)
                {
                    Vector2 texCoords = new Vector2((float)x / sizeX, (float)z / sizeZ);

                    Vector2 position = (texCoords * 2 - new Vector2(1.0f)) * noiseScale / 2;

                    float height = heightM * PerlinNoise.FBM(position.X, position.Y, 5, 2.0f, 0.5f)
                        * PerlinNoise.Noise(position.X / 10, position.Y / 10)
                        / Math.Clamp(MathF.Pow(position.Length, 0.1f), 1f, 3f);

                    vertices.Add(new Vertex(new Vector3(x * cellSize, height, z * cellSize), texCoords));

                    if (x > 0)
                    {
                        if (z < sizeZ - 1)
                            triangles.Add(new Triangle((uint)(vertices.Count - 1), (uint)(vertices.Count - 1 - sizeZ), (uint)(vertices.Count - sizeZ)));

                        if (z > 0)
                            triangles.Add(new Triangle((uint)(vertices.Count - 1), (uint)(vertices.Count - 1 - sizeZ), (uint)(vertices.Count - 2)));
                    }
                }

            NormalCalculationParams normalCalculationParams = new NormalCalculationParams();
            normalCalculationParams.autoNormalComputeForVertexNormals = true;
            normalCalculationParams.triangleNormalMode = TriangleNormalCalculationMode.FromDirection;
            normalCalculationParams.preferedNormalDirection = Vector3.UnitY;

            meshData = new MeshData(vertices, triangles, normalCalculationParams);
        }
    }
}
