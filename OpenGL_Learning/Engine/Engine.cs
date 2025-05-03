using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenGL_Learning.Engine.Objects;
using System.Runtime.InteropServices;
using OpenGL_Learning.Engine.Objects.Player;
using OpenTK.Graphics;


namespace OpenGL_Learning.Engine
{
    public enum RenderingMethod { Raster, RasterWithPostProcessing, RayTracing }

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
        public RenderingMethod renderingMethod = RenderingMethod.Raster;

        // Buffering
        int framebuffer = -1;
        Texture sceneColorTexture = null;
        Texture sceneDepthTexture = null;

        // Post processing
        public string postProcessShader = "";

        // Ray tracing
        public string rayTracingComputeShader = "";
        private int lightsSSBO = 0;
        private int cameraSSBO = 0;
        private int meshSSBO = 0;

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


            // Post processing setup
            if (renderingMethod == RenderingMethod.RasterWithPostProcessing)
            {
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

                renderPlane = new MeshObject(this, "ENGINE_RenderPlane_M", postProcessShader, new string[] { "ENGINE_SceneColor_T", "ENGINE_SceneDepth_T" });
            }

            // Ray tracing setup
            else if (renderingMethod == RenderingMethod.RayTracing)
            {     
                sceneColorTexture = new Texture(windowWidth, windowHeight, TextureType.ComputeShaderOutput);
                AddTexture("ENGINE_SceneColor_T", sceneColorTexture);

                // Initiating render plane
                AddMeshData("ENGINE_RenderPlane_M", new RenderPlaneMesh());

                // Registering default shader
                AddShader("ENGINE_RayTracingRenderPlane_S", new RenderShader(this,
                    "../../../Engine/Rendering/DefaultShaders/RayTracingRenderPlane/DefaultRayTracingRenderPlaneShader.vert",
                    "../../../Engine/Rendering/DefaultShaders/RayTracingRenderPlane/DefaultRayTracingRenderPlaneShader.frag"));

                renderPlane = new MeshObject(this, "ENGINE_RenderPlane_M", "ENGINE_RayTracingRenderPlane_S", new string[] { "ENGINE_SceneColor_T" });


                // Generating SSBOs

                // Camera SSBO
                cameraSSBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, cameraSSBO);

                CameraData initialCameraData = new CameraData();
                GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf<CameraData>(), ref initialCameraData, BufferUsageHint.DynamicDraw);
                
                GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, cameraSSBO); // Binding camera SSBO to "slot" 0

                //lightsSSBO = GL.GenBuffer();
                //meshSSBO = GL.GenBuffer();

            }

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
            if (renderingMethod == RenderingMethod.RasterWithPostProcessing)
            {
                // Binding frame buffer
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            }


            // Draw calls for all objects in the scene (used for raster rendering)
            if (  renderingMethod == RenderingMethod.RasterWithPostProcessing
                || renderingMethod == RenderingMethod.Raster)
            {
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
            }


            // Rendering with ray tracing
            else if (renderingMethod == RenderingMethod.RayTracing)
            {
                // Dispatching the compute shader
                ComputeShader rayTracingShader = (ComputeShader)shaders[rayTracingComputeShader];


                // Sending data to SSBOs

                // Camera SSBO
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, cameraSSBO);
                CameraData cameraData = GetCurrentCameraData();
                GL.BufferSubData(BufferTarget.ShaderStorageBuffer, 0, Marshal.SizeOf<CameraData>(), ref cameraData);

                // Binding output image
                GL.BindImageTexture(0, textures["ENGINE_SceneColor_T"].textureHandle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);


                rayTracingShader.UseShader(); 
                
                // Dispatching compute shader
                int groupSizeX = 16;
                int groupSizeY = 16;

                int groupsX = (windowWidth + groupSizeX - 1) / groupSizeX;
                int groupsY = (windowHeight + groupSizeY - 1) / groupSizeY;

                rayTracingShader.DispatchShader(groupsX, groupsY, MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            }


            // Drawing the render plane
            if (renderingMethod == RenderingMethod.RasterWithPostProcessing
                 || renderingMethod == RenderingMethod.RayTracing)
            {
                // Back to screen rendering
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                // Clearing old stuff on screen
                GL.ClearColor(0.0f, 0.0f, 0f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Rendering render plane to screen
                renderPlane.Render(currentWorld.worldCamera);
            }
        }

        CameraData GetCurrentCameraData()
        {
            CameraData cameraData = new CameraData();

            if (currentWorld == null) return cameraData;
            Camera camera = currentWorld.worldCamera;

            // Setting data
            cameraData.location = camera.location;

            cameraData.forwardVector = camera.forwardVector;
            cameraData.rightVector = camera.rightVector;
            cameraData.upVector = camera.upVector;

            cameraData.fieldOfView = MathHelper.DegreesToRadians(camera.fov);
            cameraData.aspectRatio = (float)windowWidth / windowHeight;

            return cameraData;
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
