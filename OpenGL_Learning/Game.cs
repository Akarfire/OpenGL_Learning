using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.meshObjects;

namespace Program
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
            int world = engine.CreateWorld();

            engine.GetWorld(world).AddObject(new CubeObject(engine, engine.GetShader(defaultShader), new Texture[] { engine.GetTexture(woodTexture) }));
            engine.GetWorld(world).AddObject(new PlaneObject(engine, engine.GetShader(defaultShader), new Texture[] { engine.GetTexture(aTexture) }));
            engine.GetWorld(world).AddObject(new GridObject(100, 100, 1, engine, engine.GetShader(waterShader), new Texture[] { engine.GetTexture(waterTexture) }));

            // Objects

            objects[1].AddWorldOffset(new Vector3(-1f, 3f, -3f));
            objects[2].AddWorldOffset(new Vector3(0f, -3f, -5f));

            objects[0].SetWorldLocation(new Vector3(1f, 3f, -10f));
            objects[0].SetScale(new Vector3(5f, 5f, 5f));
        }
    }
}
