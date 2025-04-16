using OpenTK.Graphics.Wgl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine
{
    internal class Engine
    {
        private GameWindow gW;

        public Dictionary<String, Shader> shaders { get; private set; } = new Dictionary<string, Shader>();
        public Dictionary<String, Texture> textures { get; private set; } = new Dictionary<string, Texture>();


        public Engine(GameWindow inGameWindow) { gW = inGameWindow; }


        public void OnLoad() 
        {
            //camera = new Camera(width, height, Vector3.Zero);
            gW.CursorState = CursorState.Grabbed;
        }

        public void OnUnload() { }

        public void Update(float deltaTime) { }

        public void RenderWorld(float deltaTime) { }

        public void UpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState) { }


        public void AddShader(String name, Shader shader) { shaders.Add(name, shader); }
        public void RemoveShader(String name) 
        { 
            if (shaders.ContainsKey(name))
            {
                shaders[name].DeleteShader();
                shaders.Remove(name);
            }
        }

        public void AddTexture(String name, Texture texture) { textures.Add(name, texture); }
        public void RemoveTexture(String name)
        {
            if (textures.ContainsKey(name))
            {
                textures[name].DeleteTexture();
                textures.Remove(name);
            }
        }
    }
}
