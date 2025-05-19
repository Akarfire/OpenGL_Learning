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
using OpenGL_Learning.Engine.Objects.GeneralPrimitives;
using OpenGL_Learning.Engine.Utilities;

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

            engine.windowWidth = 1280;
            engine.windowHeight = 720;

            // Loading assets

            // Shaders
            engine.AddShader("RayTracing_S", new ComputeShader(engine, shaderFolder + "RayTracing/RayTracing.comp"));

            // Meshes
            engine.AddMeshData("Cube_Mesh", new CubeMesh());
            engine.AddMeshData("Shooter_mesh", new MeshFromFile(modelsFolder + "Zaris_Shooter.fbx"));

            engine.AddMeshData("Cave_Mesh", new MeshFromFile(modelsFolder + "RTTS_Cave.fbx"));
            engine.AddMeshData("Cliff_Mesh", new MeshFromFile(modelsFolder + "RTTS_Cliff.fbx"));
            engine.AddMeshData("Sword_Mesh", new MeshFromFile(modelsFolder + "RTTS_Sword.fbx"));

            engine.AddMeshData("Sphere_Mesh", new SphereMesh(36, 18));

            engine.AddMeshData("Plane_Mesh", new PlaneMesh());

            // Materials
            Material mat_1 = new Material();
            mat_1.color = Vector3.One * 0.5f;
            mat_1.roughness = 0.6f;
            mat_1.emissionStrength = 0;

            renderEngine.AddMaterial("mat_1", mat_1);


            Material mat_2 = new Material();
            mat_2.color = Vector3.UnitY;
            mat_2.roughness = 1;
            mat_2.emissionStrength = 0.0f;

            renderEngine.AddMaterial("mat_green", mat_2);


            Material mat_3 = new Material();
            mat_3.color = Vector3.UnitX;
            mat_3.emissionStrength = 5f;

            renderEngine.AddMaterial("mat_redGlow", mat_3);


            Material mat_4 = new Material();
            mat_4.color = new Vector3(0.1f, 0.1f, 1f);
            mat_4.roughness = 0.5f;
            mat_4.emissionStrength = 0;
            mat_4.transparency = 0.8f;
            mat_4.refractionIndex = 1.3f;

            renderEngine.AddMaterial("mat_water", mat_4);


            Material mat_glass = new Material();
            mat_glass.color = Vector3.One;
            mat_glass.roughness = 0.1f;
            mat_glass.emissionStrength = 0;
            mat_glass.transparency = 0.8f;
            mat_glass.refractionIndex = 1.05f;
            mat_glass.emissionStrength = 0.1f;

            renderEngine.AddMaterial("mat_glass", mat_glass);


            Material mat_whiteGlow = new Material();
            mat_whiteGlow.color = Vector3.One;
            mat_whiteGlow.emissionStrength = 15;

            renderEngine.AddMaterial("mat_whiteGlow", mat_whiteGlow);



            Material mat_terrain = new Material();
            mat_terrain.color = Vector3.One * 1f;
            mat_terrain.roughness = 1f;

            renderEngine.AddMaterial("mat_terrain", mat_terrain);


            Material mat_cave = new Material();
            mat_cave.color = Vector3.One * 0.3f;
            mat_cave.roughness = 1;

            renderEngine.AddMaterial("mat_cave", mat_cave);


            Material mat_sword = new Material();
            mat_sword.color = Vector3.One * 0.5f;
            mat_sword.roughness = 0.3f;
            mat_sword.metallic = 1;

            renderEngine.AddMaterial("mat_sword", mat_sword);


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

            sun.SetRotation(new Vector3(0, 15, 15));


            // Cube
            //RayTracingMeshObject cube2 = new RayTracingMeshObject(engine, "Cube_Mesh");
            //cube2.materialID = 3;
            //world.AddObject(cube2);

            //cube2.AddLocation(Vector3.UnitX * 5);


            // Sphere
            //RayTracingMeshObject sphere = new RayTracingMeshObject(engine, "Shooter_mesh");
            //sphere.materialID = 2;
            //world.AddObject(sphere);

            //sphere.SetScale(Vector3.One * 5);

            //Plane
            RayTracingMeshObject plane = new RayTracingMeshObject(engine, "Plane_Mesh");
            plane.SetMaterial("mat_water");

            world.AddObject(plane);
            plane.SetScale(Vector3.One * 300);
            plane.AddRotation(Vector3.UnitX * 90);
            plane.SetLocation(Vector3.UnitY * 3);


            //RayTracingMeshObject plane_2 = new RayTracingMeshObject(engine, "Plane_Mesh");
            //plane_2.materialID = 4;

            //world.AddObject(plane_2);
            //plane_2.AddLocation(Vector3.UnitY * 5);
            //plane_2.SetScale(Vector3.One * 50);
            //plane_2.AddRotation(Vector3.UnitX * 65);


            // Terrain
            BVHProceduralTerrainObject terrain = new BVHProceduralTerrainObject(50, 50, 4f, 3, 60, engine);
            world.AddObject(terrain);

            terrain.SetMaterial("mat_terrain");

            terrain.SetLocation(new Vector3(-100f, -12, -100f));


            // Scene meshes
            RayTracingMeshObject cave = new RayTracingMeshObject(engine, "Cave_Mesh");
            world.AddObject(cave);

            cave.SetMaterial("mat_cave");

            cave.SetLocation(new Vector3(-30, 8, 0));
            cave.SetScale(Vector3.One * 5);
            cave.SetRotation(new Vector3(-90, 0, 0));


            RayTracingMeshObject cliff = new RayTracingMeshObject(engine, "Cliff_Mesh");
            world.AddObject(cliff);

            cliff.SetMaterial("mat_green");

            cliff.SetLocation(new Vector3(0, 8, 0));
            cliff.SetScale(Vector3.One * 5);
            cliff.SetRotation(new Vector3(-90, 0, 0));


            RayTracingMeshObject sword = new RayTracingMeshObject(engine, "Sword_Mesh");
            world.AddObject(sword);

            sword.SetMaterial("mat_sword");

            sword.SetLocation(new Vector3(0, 8, 0));
            sword.SetScale(Vector3.One * 5);
            sword.SetRotation(new Vector3(-90, 90, 0));


            SpherePrimitive glassSphere = new SpherePrimitive(engine, 10);
            world.AddObject(glassSphere);

            glassSphere.SetMaterial("mat_glass");
            glassSphere.SetLocation(Vector3.UnitY * 8);

            SpherePrimitive solidSphere = new SpherePrimitive(engine, 4);
            world.AddObject(solidSphere);

            solidSphere.SetMaterial("mat_1");
            solidSphere.SetLocation(Vector3.UnitY * 8);



            // Spawning primitive spheres over the terrain

            int sphereCount = 20;
            for (int i = 0; i < sphereCount; i++)
            {
                SpherePrimitive sphere = new SpherePrimitive(engine, 0.5f);
                world.AddObject(sphere);

                sphere.SetMaterial("mat_whiteGlow");

                sphere.SetLocation(GeometryUtilities.GetRandomVertexPosition(cave));
            }

            int terrainSphereCount = 50;
            for (int i = 0; i < terrainSphereCount; i++)
            {
                SpherePrimitive sphere = new SpherePrimitive(engine, 1f);
                world.AddObject(sphere);

                sphere.SetMaterial("mat_redGlow");

                sphere.SetLocation(GeometryUtilities.GetRandomVertexPosition(terrain));
            }



            engine.currentWorld.worldCamera.AddLocation(Vector3.UnitY * 20);

            engine.StartEngine();
        }
    }
}
