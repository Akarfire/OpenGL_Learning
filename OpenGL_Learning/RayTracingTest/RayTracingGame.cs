using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Objects.MeshObjects;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Objects;

namespace RayTracingTest
{
    internal class RayTracingGame
    {
        Engine engine = new Engine();

        // Folder paths
        public string shaderFolder = "../../../Shaders/";
        public string textureFolder = "../../../Textures/";
        public string modelsFolder = "../../../Models/";

        public RayTracingGame()
        {
            // Loading assets
            engine.AddShader("RayTracing_S", new ComputeShader(engine, shaderFolder + "RayTracing/RayTracing_1.comp"));

            // Ray tracing setup
            engine.renderingMethod = RenderingMethod.RayTracing;
            engine.rayTracingComputeShader = "RayTracing_S";

            // Creating world
            World world = engine.CreateWorld("MyWorld");

            engine.StartEngine();
        }
    }
}
