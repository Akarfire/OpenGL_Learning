using OpenTK.Graphics.Wgl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine
{
    public class Engine
    {
        // Modules
        private GameEngineWindow gameWindow = null;

        // Entity lists
        public List<Shader> shaders { get; private set; } = new List<Shader>();
        public List<Texture> textures { get; private set; } = new List<Texture>();
        public List<World> worlds { get; private set; } = new List<World>();


        // Window settings
        public int windowWidth { get; private set; } = 1920;
        public int windowHeight { get; private set; } = 1080;

        public bool cursorGrabbed { get; private set; } = false;


        // State
        public World currentWorld { get; private set; } = null;


        // ---------------

        public Engine() { }


        // General
        public void StartEngine() 
        {
            GL.Enable(EnableCap.DepthTest);
        }

        public void ShutdownEngine() 
        {
            if (gameWindow != null) gameWindow.Close();

            foreach (Shader shader in shaders) { shader.DeleteShader(); }
            foreach (Texture texture in textures) { texture.DeleteTexture(); }
            foreach (World world in worlds) { world.OnDestroy(); }
        }

        public void Update(float deltaTime) { }

        public void Render(float deltaTime) 
        {
            if (currentWorld == null) return;

            // Updating shaders before rendering
            foreach (Shader shader in shaders)
            {
                // Engine-level shader uniforms

                // World time uniform
                int timeLocation = GL.GetUniformLocation(shader.GetHandle(), "time");
                if (timeLocation != -1)
                    GL.Uniform1(timeLocation, currentWorld.time);
            }

            // Rendering current world
            currentWorld.RenderWorld(deltaTime);
        }

        public void UpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState) { }


        // Window management

        // Creates a new game window if one isn't already present
        public void CreateWindow()
        {
            if (gameWindow != null) return;
                
            gameWindow = new GameEngineWindow(this);
            
            GrabCursor();
        }

        public void OnWindowResized(int newWidth, int newHeight) 
        {
            windowWidth = newWidth;
            windowHeight = newHeight;

            if (currentWorld != null) { currentWorld.worldCamera.UpdateWindowSize(windowWidth, windowHeight); }
        }

        public void GrabCursor() 
        {
            gameWindow.CursorState = CursorState.Grabbed;
            cursorGrabbed = true;
        }
        public void ShowCursor() 
        { 
            gameWindow.CursorState = CursorState.Normal;
            cursorGrabbed = false;
        }


        // Registers a new shader in the engine, returns in-engine shader handle (not openGL handle) 
        public int AddShader(Shader shader) { shaders.Add(shader); return shaders.Count - 1; }

        // Removes a shader from the engine's registry by it's in-engine shader handle (not openGL handle) 
        public void RemoveShader(int handle) 
        {
            if (shaders.Count <= handle) return;

            shaders[handle].DeleteShader();
            shaders.RemoveAt(handle);
        }

        public Shader GetShader(int handle) { return shaders[handle]; }


        // Registers a new texture in the engine, returns in-engine texture handle (not openGL handle) 
        public int AddTexture(Texture texture) { textures.Add(texture); return textures.Count - 1; }

        // Removes a texture from the engine's registry by it's in-engine texture handle (not openGL handle) 
        public void RemoveTexture(int handle)
        {
            if (textures.Count <= handle) return;

            textures[handle].DeleteTexture();
            textures.RemoveAt(handle);
        }

        public Texture GetTexture(int handle) { return textures[handle]; }


        // Creates a new game world and returns it's handle
        public int CreateWorld() 
        { 
            worlds.Add(new World(this)); 
            worlds[worlds.Count - 1].OnLoad(); 
            
            return worlds.Count - 1; 
        }

        // Destroys a game world with a given handle
        public void DestroyWorld(int handle) 
        {
            if (worlds.Count <= handle) return;

            if (currentWorld == worlds[handle]) currentWorld = null;

            worlds[handle].OnDestroy();
            worlds.RemoveAt(handle);
        }

        public World GetWorld(int handle) { return worlds[handle]; }
    }
}
