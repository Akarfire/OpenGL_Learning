using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Objects.MeshObjects;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Objects.Lights;
using System.Drawing;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenGL_Learning.RayTracingTest;

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

            // Creating objects

            // Point light
            //PointLightObject pointLight = new PointLightObject(engine, 20.0f, Color.White, 0.025f);
            //world.AddObject(pointLight);

            //LightColorChangerScript lightColorChangerScript = new LightColorChangerScript();
            //pointLight.AddScript("ColorChanger", lightColorChangerScript);


            // Sun 1
            DirectionalLightObject sun = new DirectionalLightObject(engine, 1.0f, Color.White, 0.025f);
            world.AddObject(sun);
            //SunRotationScript sunRotation = new SunRotationScript();
            //sun.AddScript("rotation", sunRotation);

            sun.SetRotation(new Vector3(0, 15, 25));


            engine.StartEngine();
        }
    }
}
