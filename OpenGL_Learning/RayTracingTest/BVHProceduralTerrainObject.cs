using OpenGL_Learning.Engine.Objects;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Rendering.Mesh;
using GameCode;


namespace RayTracingTest
{
    internal class BVHProceduralTerrainObject : RayTracingMeshObject
    {
        protected int sizeX, sizeZ;
        protected float cellSize;
        protected float noiseScale;
        public BVHProceduralTerrainObject(int inSizeX, int inSizeZ, float inCellSize, float inNoiseScale, float heightM, Engine inEngine) : base(inEngine, null)
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

            meshData = new RayTracingMeshData(vertices, triangles, normalCalculationParams);
            engine.AddMeshData("ENGINE_TerrainMesh", meshData);
        }
    }
}
