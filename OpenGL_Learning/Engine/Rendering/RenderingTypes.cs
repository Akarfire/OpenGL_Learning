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

}