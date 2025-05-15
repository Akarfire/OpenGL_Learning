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
using OpenGL_Learning.Engine.Rendering.RenderEngines;

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
            // Rendering engine
            RayTracingRenderEngine renderEngine = new RayTracingRenderEngine(engine);
            engine.renderingEngine = renderEngine;

            renderEngine.rayTracingComputeShader = "RayTracing_S";

            engine.windowWidth = 1920;
            engine.windowHeight = 1080;

            // Loading assets

            // Shaders
            engine.AddShader("RayTracing_S", new ComputeShader(engine, shaderFolder + "RayTracing/RayTracing.comp"));

            // Meshes
            engine.AddMeshData("Cube_Mesh", new CubeMesh());
            engine.AddMeshData("Shooter_mesh", new MeshFromFile(modelsFolder + "Zaris_Shooter.fbx"));
            engine.AddMeshData("Pump_mesh", new MeshFromFile(modelsFolder + "PAP_Pump.fbx"));

            engine.AddMeshData("Sphere_Mesh", new SphereMesh(36, 18));

            engine.AddMeshData("Plane_Mesh", new PlaneMesh());

            // Materials
            Material mat_1 = new Material();
            mat_1.color = Vector3.One;
            mat_1.roughness = 0.6f;
            mat_1.emissionStrength = 0;

            renderEngine.AddMaterial("mat_1", mat_1);

            Material mat_2 = new Material();
            mat_2.color = Vector3.UnitY;
            mat_2.roughness = 1;
            mat_2.emissionStrength = 0.0f;

            renderEngine.AddMaterial("mat_2", mat_2);

            Material mat_3 = new Material();
            mat_3.color = Vector3.UnitX;
            mat_3.emissionStrength = 10f;

            renderEngine.AddMaterial("mat_3", mat_3);


            // Creating world
            World world = engine.CreateWorld("MyWorld");

            // Camera settings
            world.worldCamera.sensitivity = 15f;

            // Creating objects

            RenderController renderController = new RenderController(engine);
            world.AddObject(renderController);

            // Sun 
            DirectionalLightObject sun = new DirectionalLightObject(engine, 100f, Color.White, 0.03f);
            world.AddObject(sun);

            //SunRotationScript sunRotation = new SunRotationScript();
            //sun.AddScript("rotation", sunRotation);

            sun.SetRotation(new Vector3(0, 15, 25));


            //Cubes
            //MeshObject cube = new RayTracingMeshObject(engine, "Cube_Mesh");
            //world.AddObject(cube);

            RayTracingMeshObject cube2 = new RayTracingMeshObject(engine, "Cube_Mesh");
            cube2.materialID = 3;
            world.AddObject(cube2);

            cube2.AddLocation(Vector3.UnitX * 5);

            RayTracingMeshObject cube3 = new RayTracingMeshObject(engine, "Cube_Mesh");
            cube3.materialID = 2;
            world.AddObject(cube3);

            cube3.AddLocation(Vector3.UnitX * 10);


            //// Shooter
            //MeshObject shooter = new MeshObject(engine, "Shooter_mesh");
            //world.AddObject(shooter);

            // Sphere
            RayTracingMeshObject sphere = new RayTracingMeshObject(engine, "Shooter_mesh");
            sphere.materialID = 2;
            world.AddObject(sphere);

            sphere.SetScale(Vector3.One * 5);

            // Plane
            RayTracingMeshObject plane = new RayTracingMeshObject(engine, "Plane_Mesh");
            plane.materialID = 1;

            world.AddObject(plane);
            plane.SetScale(Vector3.One * 100);
            plane.AddRotation(Vector3.UnitX * 90);

            //shooter.AddLocation(Vector3.UnitX * 5);


            engine.StartEngine();
        }
    }
}
