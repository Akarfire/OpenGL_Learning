using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace OpenGL_Learning.Engine.Rendering
{

    // Contains camera data, that is sent to a compute shader via a SSBO
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraData
    {
        // MUST EXACTYLE MATCH THE SHADER SIDE

        /*
         * We have to add paddings to each vec3, as they are only 12 bytes of data, but read as 16 bytes (as if it is vec4)
         * paddints help fix the allignment when writing data from CPU to SSBO
         */

        // Location of the camera in world space
        public Vector3 location; float pad1;

        // Directional vectors of the camera
        public Vector3 forwardVector; float pad2;
        public Vector3 rightVector; float pad3;
        public Vector3 upVector; float pad4;

        // Camera's FOV
        public float fieldOfView;

        // Screen aspect ratio (width / height)
        public float aspectRatio;
    }


    // Contains data about a single light source, that will be sent to the compute shader via a SSBO
    [StructLayout(LayoutKind.Sequential)]
    public struct LightData
    {
        // Light's location in world space
        public Vector3 location;

        /*
        Type of the light:
            '0' - point light;
            '1' - directional light;
         */
        public int type;

        // Color of the light
        public Vector3 lightColor;

        // Intensity of the light
        public float intensity;

        // Forward vector of the light object
        public Vector3 direction;

        // Softness of the light
        public float softness;
    }


    // Contains data about surface material
    [StructLayout(LayoutKind.Sequential)]
    public struct Material
    {
        // Color of the surface
        public Vector3 color = Vector3.Zero;

        // Roughness of the surface
        public float roughness = 0.5f;


        // The strength of the light emited by this material
        public float emissionStrength = 0;


        // How metallic the surface is
        public float metallic = 0;



        // Transparency of the surface (0 - opaque, 1 - transparent)
        public float transparency = 0;

        // Index of refraction of transparent materials
        public float refractionIndex = 1;

        public Material() { }
    }


    // Contains data about a single triangle
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTriangle
    {
        public Vector3 v1; public float normalX;
        public Vector3 v2; public float normalY;
        public Vector3 v3; public float normalZ;
    };


    // Contains data about a single object
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectData
    {
        // Material ID that will be applied to all triangles of this object
        public int materialID;

        // Range of indices of trinagles, that belong to this object
        public int trianglesStart;
        public int trianglesEndOffset;

        int pad;
    };


    // Constains data about a single BVH node
    [StructLayout(LayoutKind.Sequential)]
    public struct BVH_Node
    {
        /*
        Center and extent are used for collision testing the boudning box

        lIndex and rIndex can mean two things:
            * If they are positive: they are pointing to the child nodes (as offsets from the root)
            * If they are negative: their absolute values mean: start and endOffset of the corresponding trianlges (LEAF CASE)
        */

        public Vector3 center; public int lIndex;
        public Vector3 extent; public int rIndex;
    }

}