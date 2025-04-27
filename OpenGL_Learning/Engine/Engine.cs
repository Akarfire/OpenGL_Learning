using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenGL_Learning.Engine.Objects;


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
        public int windowWidth { get; private set; } = 2160;
        public int windowHeight { get; private set; } = 1080;

        public bool cursorGrabbed { get; private set; } = false;


        // State
        public World currentWorld { get; private set; } = null;

        public KeyboardState cachedKeyboardState { get; private set; } = null;
        public MouseState cachedMouseState { get; private set; } = null;


        // Rendering

        // Buffering
        int framebuffer = -1;
        Texture sceneColorTexture = null;
        Texture sceneDepthTexture = null;

        public bool UsePostProcessing = false;
        public string postProcessShader = "";

        // Plane, to which the scene is going to be rendered (if post processing is enabled)
        MeshObject renderPlane = null;

        // ---------------

        public Engine() { CreateWindow(); }


        // General
        public void StartEngine() 
        {
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            // Creating a frame buffer
            framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            // Binding scene color texture to framebuffer (scene will be rendered here)
            sceneColorTexture = new Texture(windowWidth, windowHeight);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, sceneColorTexture.textureHandle, 0);

            // Binding scene depth texture to framebuffer
            sceneDepthTexture = new Texture(windowWidth, windowHeight, TextureType.DepthMap);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, sceneDepthTexture.textureHandle, 0);

            // Modifying depth texture parameters
            sceneDepthTexture.UseTexture(TextureUnit.Texture0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)All.None);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Specifying draw/read buffers
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

            // Error checking
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("ERROR: Failed creating frame buffer");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            // Registering textures
            AddTexture("ENGINE_SceneColor_T", sceneColorTexture);
            AddTexture("ENGINE_SceneDepth_T", sceneDepthTexture);

            // Initiating render plane
            AddMeshData("ENGINE_RenderPlane_M", new RenderPlaneMesh());

            //"ENGINE_SceneColor_T", "ENGINE_SceneDepth_T"
            renderPlane = new MeshObject(this, "ENGINE_RenderPlane_M", postProcessShader, new string[] { "ENGINE_SceneColor_T", "ENGINE_SceneDepth_T" });
            

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
            if (UsePostProcessing)
            {
                // Binding frame buffer
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            }

            // Clearing old stuff in the frame buffer
            GL.ClearColor(0.0f, 0.0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (currentWorld == null) return;

            // Engine-level shader uniforms
            foreach (var shaderPair in shaders)
            {
                var shader = shaderPair.Value;

                shader.UseShader();

                shader.SetUniform("time", currentWorld.time);

                shader.SetUniform("light_direction", currentWorld.lightDirection);
                shader.SetUniform("ambient_light", 0.5f);

                shader.SetUniform("screen_width", (float)windowWidth);
                shader.SetUniform("screen_height", (float)windowHeight);

                shader.SetUniform("camera_location", currentWorld.worldCamera.location);
                shader.SetUniform("camera_vector", currentWorld.worldCamera.forwardVector);

                shader.SetUniform("view", currentWorld.worldCamera.GetViewMatrix(), true);
                shader.SetUniform("projection", currentWorld.worldCamera.GetProjectionMatrix(), true);

                shader.SetUniform("view_inverse", currentWorld.worldCamera.GetViewMatrix().Inverted(), true);
                shader.SetUniform("projection_inverse", currentWorld.worldCamera.GetProjectionMatrix().Inverted(), true);

                shader.StopUsingShader();
            }

            // Rendering current world
            currentWorld.RenderWorld(deltaTime);
            
            if (UsePostProcessing)
            {
                // Back to screen rendering
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                // Clearing old stuff on screen
                GL.ClearColor(0.0f, 0.0f, 0f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Rendering screen to render plane
                renderPlane.Render(currentWorld.worldCamera);
            }
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
