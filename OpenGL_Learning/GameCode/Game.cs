using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.objects.meshObjects;
using OpenGL_Learning.Engine.Scripts.EngineScripts;
using OpenTK.Mathematics;

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
            // Loading shaders
            engine.AddShader("Default_S", new Shader(shaderFolder + "shader.vert", shaderFolder + "shader.frag"));
            engine.AddShader("Water_S", new Shader(shaderFolder + "shaderWater.vert", shaderFolder + "shaderWater.frag"));

            // Loading textures
            engine.AddTexture("Wood_T", new Texture(textureFolder + "wood.jpg"));
            engine.AddTexture("Water_T", new Texture(textureFolder + "sea-water-512x512.png"));
            engine.AddTexture("a_T", new Texture(textureFolder + "a.png"));


            // Creating world and objects
            World world = engine.CreateWorld("MyWorld");


            CubeObject cube = new CubeObject(engine, "Default_S", new string[] { "Wood_T" });
            world.AddObject(cube);

            PhysicsScript cubePhysics = new PhysicsScript();
            cubePhysics.enableGravityForce = true;
            cube.AddScript("Physics", cubePhysics);

            WaterBouancyScript cubeBouancy = new WaterBouancyScript();
            cube.AddScript("Bouancy", cubeBouancy);


            PlaneObject plane = new PlaneObject(engine, "Default_S", new string[] { "a_T" });
            world.AddObject(plane);


            GridObject waterGrid = new GridObject(100, 100, 1, engine, "Water_S", new string[] { "Water_T" });
            world.AddObject(waterGrid);

            // Objects

            plane.AddLocation(new Vector3(-1f, 3f, -3f));
            waterGrid.SetLocation(new Vector3(-50f, -3f, -50f));

            world.worldCamera.AddLocation(new Vector3(0, 10, 0));

            cube.SetLocation(world.worldCamera.location + world.worldCamera.forwardVector * 20);
            cube.SetScale(new Vector3(5f, 5f, 5f));


            // Starting the engine
            engine.StartEngine();
        }
    }
}
