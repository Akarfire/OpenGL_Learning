using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Objects.MeshObjects;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Objects;


namespace GameCode
{
    internal class Game
    {
        Engine engine = new Engine();

        // Folder paths
        public string shaderFolder = "../../../Shaders/";
        public string textureFolder = "../../../Textures/";

        public Game()
        {
            // Loading meshes
            engine.AddMeshData("Cube_M", new CubeMesh());
            engine.AddMeshData("Sphere_M", new SphereMesh());
            engine.AddMeshData("Ship_M", new MeshFromFile("D:\\3D_Models\\Exported\\Zaris\\Zaris_Shooter.fbx"));

            // Loading shaders
            engine.AddShader("Default_S", new Shader(engine, shaderFolder + "Objects\\Default\\DefaultShader.vert", shaderFolder + "Objects\\Default\\DefaultShader.frag"));
            engine.AddShader("Water_S", new Shader(engine, shaderFolder + "Objects\\Water\\WaterShader.vert", shaderFolder + "Objects\\Water\\WaterShader.frag"));
            engine.AddShader("Default_PPS", new Shader(engine, shaderFolder + "PostProcessing\\Default\\DefaultPostProcessingShader.vert", shaderFolder + "PostProcessing\\Default\\DefaultPostProcessingShader.frag"));
            engine.AddShader("Fog_PPS", new Shader(engine, shaderFolder + "PostProcessing\\Fog\\Fog_PPS.vert", shaderFolder + "PostProcessing\\Fog\\Fog_PPS.frag"));
            engine.AddShader("Sky_S", new Shader(engine, shaderFolder + "Objects\\Sky\\SkyShader.vert", shaderFolder + "Objects\\Sky\\SkyShader.frag"));

            // Loading textures
            engine.AddTexture("Wood_T", new Texture(textureFolder + "wood.jpg"));
            engine.AddTexture("Water_T", new Texture(textureFolder + "sea-water-512x512.png"));
            engine.AddTexture("a_T", new Texture(textureFolder + "a.png"));
            engine.AddTexture("Shooter_T", new Texture(textureFolder + "Shooter_ColorMap.png"));


            // Enabling post processing
            engine.UsePostProcessing = true;
            engine.postProcessShader = "Fog_PPS";

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
            GridObject waterGrid = new GridObject(100, 100, 2, engine, "Water_S", new string[] { "Water_T" });
            world.AddObject(waterGrid);

            waterGrid.IsTranparent = true;

            waterGrid.SetLocation(new Vector3(-100f, 0f, -100f));


            // Cube
            MeshObject cube = new MeshObject(engine, "Cube_M", "Default_S", new string[] { "Wood_T" });
            world.AddObject(cube);

            PhysicsScript cubePhysics = new PhysicsScript();
            cubePhysics.enableGravityForce = true;
            cubePhysics.dragForceStrenght = 0.5f;
            cube.AddScript("Physics", cubePhysics);

            WaterBouancyScript cubeBouancy = new WaterBouancyScript();
            cube.AddScript("Bouancy", cubeBouancy);

            cube.SetLocation(new Vector3(0, 1, 0));
            cube.SetScale(new Vector3(5f, 5f, 5f));


            // Player ship
            PlayerShip ship = new PlayerShip(engine);
            world.AddObject(ship);

            ship.SetLocation(new Vector3(5, 1, 5));


            // Camera settings
            world.worldCamera.AddLocation(new Vector3(0, 10, 0));
            world.worldCamera.maxViewDistance = 400f;
            //world.worldCamera.speed = 60f;

            // Starting the engine
            engine.StartEngine();
        }
    }
}
