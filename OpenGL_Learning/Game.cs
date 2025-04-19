using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.meshObjects;
using OpenTK.Mathematics;

namespace ProgramNameSpace
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
            int defaultShader = engine.AddShader(new Shader(shaderFolder + "shader.vert", shaderFolder + "shader.frag"));
            int waterShader = engine.AddShader(new Shader(shaderFolder + "shaderWater.vert", shaderFolder + "shaderWater.frag"));

            // Loading textures
            int woodTexture = engine.AddTexture(new Texture(textureFolder + "wood.jpg"));
            int waterTexture = engine.AddTexture(new Texture(textureFolder + "sea-water-512x512.png"));
            int aTexture = engine.AddTexture(new Texture(textureFolder + "a.png"));

            // Creating world and objects
            World world = engine.GetWorld(engine.CreateWorld());


            CubeObject cube = new CubeObject(engine, defaultShader, new int[] { woodTexture });
            PlaneObject plane = new PlaneObject(engine, defaultShader, new int[] { aTexture });
            GridObject waterGrid = new GridObject(100, 100, 1, engine, waterShader, new int[] { waterTexture });

            world.AddObject(cube);
            world.AddObject(plane);
            world.AddObject(waterGrid);
            
            // Objects

            plane.AddLocation(new Vector3(-1f, 3f, -3f));
            waterGrid.SetLocation(new Vector3(-50f, -3f, -50f));

            cube.SetLocation(new Vector3(1f, 3f, -10f));
            cube.SetScale(new Vector3(5f, 5f, 5f));


            engine.StartEngine();
        }
    }
}
