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
            engine.AddMeshData("Ship_M", new MeshFromFile("D:\\3D_Models\\Exported\\Zaris\\Zaris_Shooter.fbx"));

            // Loading shaders
            engine.AddShader("Default_S", new Shader(engine, shaderFolder + "shader.vert", shaderFolder + "shader.frag"));
            engine.AddShader("Water_S", new Shader(engine, shaderFolder + "shaderWater.vert", shaderFolder + "shaderWater.frag"));

            // Loading textures
            engine.AddTexture("Wood_T", new Texture(textureFolder + "wood.jpg"));
            engine.AddTexture("Water_T", new Texture(textureFolder + "sea-water-512x512.png"));
            engine.AddTexture("a_T", new Texture(textureFolder + "a.png"));
            engine.AddTexture("Shooter_T", new Texture(textureFolder + "Shooter_ColorMap.png"));


            // Creating world and objects
            World world = engine.CreateWorld("MyWorld");


            GridObject waterGrid = new GridObject(200, 200, 2, engine, "Water_S", new string[] { "Water_T" });
            world.AddObject(waterGrid);

            waterGrid.SetLocation(new Vector3(-200f, 0f, -200f));


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


            PlayerShip ship = new PlayerShip(engine);
            world.AddObject(ship);

            ship.SetLocation(new Vector3(5, 1, 5));


            world.worldCamera.AddLocation(new Vector3(0, 10, 0));

            // Starting the engine
            engine.StartEngine();
        }
    }
}
