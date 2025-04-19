using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.meshObjects
{
    internal class GridObject : MeshObject
    {
        protected int sizeX, sizeZ;
        protected float cellSize;
        public GridObject(int inSizeX, int inSizeZ, float inCellSize, Engine inEngine, int shaderHandle, int[] textureHandles) : base(inEngine, shaderHandle, textureHandles)
        {
            sizeX = inSizeX;
            sizeZ = inSizeZ;
            cellSize = inCellSize;

            vertices = new List<Vertex>();
            triangles = new List<Triangle>();

            for (int x = 0; x < inSizeX; x++)
                for (int z = 0; z < inSizeZ; z++)
                {
                    Vector2 texCoords = new Vector2((float)x / sizeX, (float)z / sizeZ);

                    vertices.Add(new Vertex(new Vector3(x * cellSize, 0, z * cellSize), texCoords));
                    if (x > 0)
                    {
                        if (z < sizeZ - 1)
                            triangles.Add(new Triangle((uint)(vertices.Count - 1), (uint)(vertices.Count - 1 - sizeZ), (uint)(vertices.Count - sizeZ)));

                        if (z > 0)
                            triangles.Add(new Triangle((uint)(vertices.Count - 1), (uint)(vertices.Count - 1 - sizeZ), (uint)(vertices.Count - 2)));
                    }
                }

            InitMeshObject();
        }
    }
}
