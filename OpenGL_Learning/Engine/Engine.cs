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
        public Dictionary<string, Shader> shaders { get; private set; } = new Dictionary<string, Shader>();
        public Dictionary<string, Texture> textures { get; private set; } = new Dictionary<string, Texture>();
        public Dictionary<string, World> worlds { get; private set; } = new Dictionary<string, World>();


        // Window settings
        public int windowWidth { get; private set; } = 1920;
        public int windowHeight { get; private set; } = 1080;

        public bool cursorGrabbed { get; private set; } = false;


        // State
        public World currentWorld { get; private set; } = null;

        public KeyboardState cachedKeyboardState { get; private set; } = null;
        public MouseState cachedMouseState { get; private set; } = null;


        // ---------------

        public Engine() { CreateWindow(); }


        // General
        public void StartEngine() 
        {
            GL.Enable(EnableCap.DepthTest);
            gameWindow.Run();
            
        }

        public void ShutdownEngine() 
        {
            if (gameWindow != null) gameWindow.Close();

            foreach (var shader in shaders) { shader.Value.DeleteShader(); }
            foreach (var texture in textures) { texture.Value.DeleteTexture(); }
            foreach (var world in worlds) { world.Value.OnDestroy(); }
        }

        public void Update(float deltaTime) 
        {
            if (currentWorld == null) return;

            UpdateEngineInput(deltaTime);
            currentWorld.Update(deltaTime);
        }


        public void CacheInput(KeyboardState keyboardState, MouseState mouseState)
        {
            cachedKeyboardState = keyboardState;
            cachedMouseState = mouseState;
        }

        protected void UpdateEngineInput(float deltaTime) 
        {

            // Engine-level input

            if (cachedKeyboardState.IsKeyDown(Keys.Escape))
            {
                gameWindow.Close();
            }

            if (cachedKeyboardState.IsKeyReleased(Keys.F1) && cachedKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (cursorGrabbed) ShowCursor();
                else GrabCursor();

                cursorGrabbed = !cursorGrabbed;

                if (currentWorld != null) { currentWorld.worldCamera.SetMouseInputEnabled(cursorGrabbed); }
            }
        }

        public void Render(float deltaTime)
        {
            if (currentWorld == null) return;

            // Updating shaders before rendering
            foreach (var shaderPair in shaders)
            {
                var shader = shaderPair.Value;
                // Engine-level shader uniforms

                // World time uniform
                int timeLocation = GL.GetUniformLocation(shader.GetHandle(), "time");
                if (timeLocation != -1)
                    GL.Uniform1(timeLocation, currentWorld.time);
            }

            // Rendering current world
            currentWorld.RenderWorld(deltaTime);
        }


        // Window management

        // Creates a new game window if one isn't already present
        protected void CreateWindow()
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

            GL.Viewport(0, 0, windowWidth, windowHeight);
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
        public void AddShader(string shaderName, Shader shader) { shaders.Add(shaderName, shader);}

        // Removes a shader from the engine's registry by it's in-engine shader handle (not openGL handle) 
        public void RemoveShader(string shaderName) 
        {
            if (!shaders.ContainsKey(shaderName)) return;

            shaders[shaderName].DeleteShader();
            shaders.Remove(shaderName);
        }


        // Registers a new texture in the engine, returns in-engine texture handle (not openGL handle) 
        public void AddTexture(string textureName, Texture texture) { textures.Add(textureName, texture);}

        // Removes a texture from the engine's registry by it's in-engine texture handle (not openGL handle) 
        public void RemoveTexture(string textureName)
        {
            if (!textures.ContainsKey(textureName)) return;

            textures[textureName].DeleteTexture();
            textures.Remove(textureName);
        }

        // Creates a new game world and returns it's handle
        public World CreateWorld(string worldName) 
        {
            World newWorld = new World(this);
            worlds.Add(worldName, newWorld);

            newWorld.OnLoad();

            if (currentWorld == null) currentWorld = newWorld;

            return newWorld;
        }

        // Destroys a game world with a given handle
        public void DestroyWorld(string worldName) 
        {
            if (!worlds.ContainsKey(worldName)) { Console.WriteLine("Error world '" + worldName + "' does not exist!"); return; }

            if (currentWorld == worlds[worldName]) currentWorld = null;

            worlds[worldName].OnDestroy();
            worlds.Remove(worldName);
        }

        public void SetCurrentWolrd(string worldName)
        {
            if (!worlds.ContainsKey(worldName)) { Console.WriteLine("Error world '" + worldName + "' does not exist!"); return; }
            currentWorld = worlds[worldName];
        }
    }
}
