using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenGL_Learning.Engine.Rendering;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenGL_Learning.Engine.Rendering.Mesh;
using OpenTK.Mathematics;


namespace OpenGL_Learning.Engine
{
    public class Engine
    {
        // Modules
        private GameEngineWindow gameWindow = null;

        // Entity lists
        public Dictionary<string, MeshData> meshes { get; private set; } = new Dictionary<string, MeshData>();
        public Dictionary<string, Shader> shaders { get; private set; } = new Dictionary<string, Shader>();
        public Dictionary<string, Texture> textures { get; private set; } = new Dictionary<string, Texture>();
        public Dictionary<string, World> worlds { get; private set; } = new Dictionary<string, World>();


        // Window settings
        public int windowWidth { get; set; } = 2160;
        public int windowHeight { get; set; } = 1080;

        public bool cursorGrabbed { get; private set; } = false;


        // State
        public World currentWorld { get; private set; } = null;

        public KeyboardState cachedKeyboardState { get; private set; } = null;
        public MouseState cachedMouseState { get; private set; } = null;

        // Files
        public string EngineFilesDirectory { get; private set; } = "../../../Engine/";


        // Rendering
        public RenderingEngine renderingEngine;


        // ---------------

        public Engine() { CreateWindow(); }


        // General
        public void StartEngine() 
        {
            gameWindow.Size = new Vector2i(windowWidth, windowHeight);

            renderingEngine.SetUp();

            // Running the window
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
            Console.Clear();
            Console.WriteLine(1 / deltaTime);

            renderingEngine.Render(deltaTime);

            // GPU DEBUGGING
            // ----

            //var error = GL.GetError();
            //Console.WriteLine(error);

            // ----
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


        // Registers new mesh data in the engine
        public void AddMeshData(string meshName, MeshData mesh) { meshes.Add(meshName, mesh);}

        // Removes mesh data from the engine's registry
        public void RemoveMeshData(string meshName) 
        {
            if (!meshes.ContainsKey(meshName)) return;

            meshes[meshName].Destroy();
            meshes.Remove(meshName);
        }

        // Registers a new shader in the engine
        public void AddShader(string shaderName, Shader shader) { shaders.Add(shaderName, shader); }

        // Removes a shader from the engine's registry
        public void RemoveShader(string shaderName)
        {
            if (!shaders.ContainsKey(shaderName)) return;

            shaders[shaderName].DeleteShader();
            shaders.Remove(shaderName);
        }

        // Registers a new texture in the engine
        public void AddTexture(string textureName, Texture texture) { textures.Add(textureName, texture);}

        // Removes a texture from the engine's registry
        public void RemoveTexture(string textureName)
        {
            if (!textures.ContainsKey(textureName)) return;

            textures[textureName].DeleteTexture();
            textures.Remove(textureName);
        }


        // Creates a new game world
        public World CreateWorld(string worldName) 
        {
            World newWorld = new World(this);
            worlds.Add(worldName, newWorld);

            newWorld.OnLoad();

            if (currentWorld == null) currentWorld = newWorld;

            return newWorld;
        }

        // Destroys a game world
        public void DestroyWorld(string worldName) 
        {
            if (!worlds.ContainsKey(worldName)) { Console.WriteLine("Error world '" + worldName + "' does not exist!"); return; }

            if (currentWorld == worlds[worldName]) currentWorld = null;

            worlds[worldName].OnDestroy();
            worlds.Remove(worldName);
        }

        // Changes current game world
        public void SetCurrentWolrd(string worldName)
        {
            if (!worlds.ContainsKey(worldName)) { Console.WriteLine("Error world '" + worldName + "' does not exist!"); return; }
            currentWorld = worlds[worldName];
        }
    }
}
