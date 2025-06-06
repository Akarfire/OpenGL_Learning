﻿using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace OpenGL_Learning.Engine.Rendering.Mesh
{
    // Contains data about a single triangle
    public struct RenderTriangleGen
    {
        public Vector3 v1; public float normalX;
        public Vector3 v2; public float normalY;
        public Vector3 v3; public float normalZ;

        public int v1Index;
        public int v2Index;
        public int v3Index;
    };

    // A structure used in the process of generating a BVH
    internal class BVHNodeGen
    {
        public Vector3 minExtent = new Vector3(float.PositiveInfinity);
        public Vector3 maxExtent = new Vector3(float.NegativeInfinity);

        public BVHNodeGen leftChild = null;
        public BVHNodeGen rightChild = null;

        public bool leaf = false;

        public Vector3 cachedAverageLocation = Vector3.Zero;

        public List<RenderTriangleGen> renderTriangles = new List<RenderTriangleGen>();

        public BVHNodeGen() { }
    }

    public class RayTracingMeshData : MeshData
    {
        public List<RenderTriangle> BVH_triangles { get; protected set; } = new List<RenderTriangle>();
        public List<Vector3> BVH_vertices { get; protected set; } = new List<Vector3>();
        public List<BVHNode> BVH_tree { get; protected set; } = new List<BVHNode>();


        public RayTracingMeshData(List<Vertex> inVertices = null, List<Triangle> inTriangles = null, NormalCalculationParams inNormalCalculationParams = default(NormalCalculationParams)) :
            base(inVertices, inTriangles, inNormalCalculationParams)
        { }


        // Called by the RayTracingRenderEngine on engine start up
        public void GenerateBVH()
        {

            List<RenderTriangleGen> tempTriangles = new List<RenderTriangleGen>();

            BVHNodeGen rootNode = new BVHNodeGen();

            for (int i = 0; i < triangles.Count; i++)
            {

                // Applying model matrix to triangles
                Triangle triangle = triangles[i];
                RenderTriangleGen newTriangle = new RenderTriangleGen();

                newTriangle.v1 = vertices[(int)triangle.v1].position;
                newTriangle.v2 = vertices[(int)triangle.v2].position;
                newTriangle.v3 = vertices[(int)triangle.v3].position;

                newTriangle.normalX = triangle.normal.X;
                newTriangle.normalY = triangle.normal.Y;
                newTriangle.normalZ = triangle.normal.Z;


                tempTriangles.Add(newTriangle);


                // Calculating initial boudning box
                rootNode.minExtent = ComponentMinV3(rootNode.minExtent, newTriangle.v1);
                rootNode.minExtent = ComponentMinV3(rootNode.minExtent, newTriangle.v2);
                rootNode.minExtent = ComponentMinV3(rootNode.minExtent, newTriangle.v3);

                rootNode.maxExtent = ComponentMaxV3(rootNode.maxExtent, newTriangle.v1);
                rootNode.maxExtent = ComponentMaxV3(rootNode.maxExtent, newTriangle.v2);
                rootNode.maxExtent = ComponentMaxV3(rootNode.maxExtent, newTriangle.v3);

                // Caching average location
                rootNode.cachedAverageLocation += (newTriangle.v1 + newTriangle.v2 + newTriangle.v3) / 3;

            }

            rootNode.renderTriangles = tempTriangles;
            rootNode.cachedAverageLocation /= triangles.Count;

            // Generating the tree
            SplitBVHNode(rootNode, 0);

            // Converting the tree into actual data, that will be sent to GPU
            BVH_triangles.Clear();
            BVH_tree.Clear();

            //BVH_triangles = tempTriangles;
            StoreBVHNode(0, rootNode, BVH_tree, BVH_triangles, BVH_vertices);
        }


        private static void SplitBVHNode(BVHNodeGen node, int xzyAxisFlip)
        {
            // Branch end condition
            if (node.renderTriangles.Count < 7) { node.leaf = true; return; }

            BVHNodeGen leftNode = new BVHNodeGen();
            BVHNodeGen rightNode = new BVHNodeGen();

            node.leftChild = leftNode;
            node.rightChild = rightNode;

            foreach (var tri in node.renderTriangles)
            {
                if (    (xzyAxisFlip == 0 && (tri.v1.Z + tri.v2.Z + tri.v3.Z) / 3 < node.cachedAverageLocation.Z)
                    ||  (xzyAxisFlip == 1 && (tri.v1.X + tri.v2.X + tri.v3.X) / 3 < node.cachedAverageLocation.X)
                    ||  (xzyAxisFlip == 2 && (tri.v1.Y + tri.v2.Y + tri.v3.Y) / 3 < node.cachedAverageLocation.Y))
                {
                    leftNode.renderTriangles.Add(tri);
                    leftNode.cachedAverageLocation += (tri.v1 + tri.v2 + tri.v3) / 3;

                    leftNode.minExtent = ComponentMinV3(leftNode.minExtent, tri.v1);
                    leftNode.minExtent = ComponentMinV3(leftNode.minExtent, tri.v2);
                    leftNode.minExtent = ComponentMinV3(leftNode.minExtent, tri.v3);

                    leftNode.maxExtent = ComponentMaxV3(leftNode.maxExtent, tri.v1);
                    leftNode.maxExtent = ComponentMaxV3(leftNode.maxExtent, tri.v2);
                    leftNode.maxExtent = ComponentMaxV3(leftNode.maxExtent, tri.v3);
                }

                else
                {
                    rightNode.renderTriangles.Add(tri);
                    rightNode.cachedAverageLocation += (tri.v1 + tri.v2 + tri.v3) / 3;

                    rightNode.minExtent = ComponentMinV3(rightNode.minExtent, tri.v1);
                    rightNode.minExtent = ComponentMinV3(rightNode.minExtent, tri.v2);
                    rightNode.minExtent = ComponentMinV3(rightNode.minExtent, tri.v3);

                    rightNode.maxExtent = ComponentMaxV3(rightNode.maxExtent, tri.v1);
                    rightNode.maxExtent = ComponentMaxV3(rightNode.maxExtent, tri.v2);
                    rightNode.maxExtent = ComponentMaxV3(rightNode.maxExtent, tri.v3);
                }
            }

            leftNode.cachedAverageLocation /= leftNode.renderTriangles.Count;
            rightNode.cachedAverageLocation /= rightNode.renderTriangles.Count;

            SplitBVHNode(leftNode, (xzyAxisFlip + 1) % 3);
            SplitBVHNode(rightNode, (xzyAxisFlip + 1) % 3);
        }

        private static int StoreBVHNode(int index, BVHNodeGen node, List<BVHNode> outNodeList, List<RenderTriangle> outTriangleList, List<Vector3> outVertexList)
        {
            BVHNode storeNode = new BVHNode();

            storeNode.minExtent = node.minExtent;
            storeNode.maxExtent = node.maxExtent;

            if (!node.leaf) 
            {
                outNodeList.Add(storeNode);
                int storeNodeIndex = outNodeList.Count - 1;

                storeNode.lIndex = index + 1;

                int tempIndex = StoreBVHNode(index + 1, node.leftChild, outNodeList, outTriangleList, outVertexList);

                storeNode.rIndex = tempIndex;

                outNodeList[storeNodeIndex] = storeNode; 

                return StoreBVHNode(tempIndex, node.rightChild, outNodeList, outTriangleList, outVertexList);
            }

            else
            {
                storeNode.lIndex = outTriangleList.Count;
                storeNode.rIndex = node.renderTriangles.Count;

                foreach( var triangle in node.renderTriangles) 
                {
                    RenderTriangle tri = new RenderTriangle();
                    tri.v1 = triangle.v1;
                    tri.v2 = triangle.v2;
                    tri.v3 = triangle.v3;

                    tri.normalX = triangle.normalX;
                    tri.normalY = triangle.normalY;
                    tri.normalZ = triangle.normalZ;

                    //tri.v1Index = outVertexList.Count;
                    //tri.v2Index = outVertexList.Count + 1;
                    //tri.v3Index = outVertexList.Count + 2;

                    outTriangleList.Add(tri);

                    outVertexList.Add(triangle.v1);
                    outVertexList.Add(triangle.v2);
                    outVertexList.Add(triangle.v3);
                }

                storeNode.lIndex *= -1;
                storeNode.rIndex *= -1;
                outNodeList.Add(storeNode);

                return index + 1;
            }
        }

        public static Vector3 ComponentMinV3(Vector3 lhs, Vector3 rhs)
        {
            Vector3 outVector = new Vector3();

            outVector.X = MathF.Min(lhs.X, rhs.X);
            outVector.Y = MathF.Min(lhs.Y, rhs.Y);
            outVector.Z = MathF.Min(lhs.Z, rhs.Z);

            return outVector;
        }

        public static Vector3 ComponentMaxV3(Vector3 lhs, Vector3 rhs)
        {
            Vector3 outVector = new Vector3();

            outVector.X = MathF.Max(lhs.X, rhs.X);
            outVector.Y = MathF.Max(lhs.Y, rhs.Y);
            outVector.Z = MathF.Max(lhs.Z, rhs.Z);

            return outVector;
        }
    }
}
