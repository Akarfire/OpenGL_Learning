using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Objects.MeshObjects;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Rendering.RenderEngines;


namespace GameCode
{
    internal class Game
    {
        Engine engine = new Engine();

        // Folder paths
        public string shaderFolder = "../../../Shaders/";
        public string textureFolder = "../../../Textures/";
        public string modelsFolder = "../../../Models/";

        public Game()
        {
            // Rendering engine setup
            RasterRenderEngine renderEngine = new RasterRenderEngine(engine);
            engine.renderingEngine = renderEngine;

            renderEngine.postProcessApplicationShaderName = "Fog_PPS";


            // Loading meshes
            engine.AddMeshData("Cube_M", new CubeMesh());
            engine.AddMeshData("Sphere_M", new SphereMesh());
            engine.AddMeshData("Ship_M", new MeshFromFile(modelsFolder + "Zaris_Shooter.fbx"));

            // Loading shaders
            engine.AddShader("Default_S", new RenderShader(engine, shaderFolder + "Objects\\Default\\DefaultShader.vert", shaderFolder + "Objects\\Default\\DefaultShader.frag"));
            engine.AddShader("Water_S", new RenderShader(engine, shaderFolder + "Objects\\Water\\WaterShader.vert", shaderFolder + "Objects\\Water\\WaterShader.frag"));
            engine.AddShader("Default_PPS", new RenderShader(engine, shaderFolder + "PostProcessing\\Default\\DefaultPostProcessingShader.vert", shaderFolder + "PostProcessing\\Default\\DefaultPostProcessingShader.frag"));
            engine.AddShader("Fog_PPS", new RenderShader(engine, shaderFolder + "PostProcessing\\Fog\\Fog_PPS.vert", shaderFolder + "PostProcessing\\Fog\\Fog_PPS.frag"));
            engine.AddShader("Sky_S", new RenderShader(engine, shaderFolder + "Objects\\Sky\\SkyShader.vert", shaderFolder + "Objects\\Sky\\SkyShader.frag"));
            engine.AddShader("Terrain_S", new RenderShader(engine, shaderFolder + "Objects\\Terrain\\TerrainShader.vert", shaderFolder + "Objects\\Terrain\\TerrainShader.frag"));

            // Loading textures
            engine.AddTexture("Wood_T", new Texture(textureFolder + "wood.jpg"));
            engine.AddTexture("Water_T", new Texture(textureFolder + "WaterStyleTex.jpg"));
            engine.AddTexture("a_T", new Texture(textureFolder + "a.png"));
            engine.AddTexture("Shooter_T", new Texture(textureFolder + "Shooter_ColorMap.png"));


            // Enabling post processing
            //engine.renderingMethod = RenderingMethod.RasterWithPostProcessing;
            //engine.postProcessShader = "Fog_PPS";

            // Creating world and objects
            World world = engine.CreateWorld("MyWorld");
            world.lightDirection = new Vector3(1, 0.35f, 1).Normalized();

            // Sky sphere
            MeshObject skySphere = new MeshObject(engine, "Sphere_M", "Sky_S", null);
            skySphere.SetScale(new Vector3(500));

            AttachmentScript skyAttachment = new AttachmentScript();
            skySphere.AddScript("CameraAttachment", skyAttachment);
            skyAttachment.attachementParent = world.worldCamera;

            SunRotation sunRotation = new SunRotation();
            skySphere.AddScript("SunRotation", sunRotation);

            world.AddObject(skySphere);


            // Water grid
            GridObject waterGrid = new GridObject(800, 800, 1, engine, "Water_S", new string[] { "Water_T" });
            world.AddObject(waterGrid);

            waterGrid.IsTranparent = true;

            AttachmentScript waterAttachment = new AttachmentScript();
            waterGrid.AddScript("CameraAttachment", waterAttachment);
            waterAttachment.attachementParent = world.worldCamera;
            waterAttachment.attachmentPositionMask = new Vector3(1, 0, 1);
            waterAttachment.attachmentPositionOffset = new Vector3(-400f, 0f, -400f);


            // Terrain
            ProceduralTerrainObject terrain = new ProceduralTerrainObject(800, 800, 1, 9, 125, engine, "Terrain_S", new string[] { "Wood_T" });
            world.AddObject(terrain);

            terrain.SetLocation(new Vector3(-400f, -40f, -400f));


            // Player ship
            PlayerShip ship = new PlayerShip(engine);
            world.AddObject(ship);

            ship.SetLocation(new Vector3(5, 1, 5));


            // Camera settings
            world.worldCamera.AddLocation(new Vector3(0, 10, 0));
            world.worldCamera.maxViewDistance = 400f;
            world.worldCamera.speed = 60f;

            // Starting the engine
            engine.StartEngine();
        }
    }
}
