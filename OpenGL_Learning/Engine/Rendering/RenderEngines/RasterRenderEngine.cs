using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_Learning.Engine.Rendering.RenderEngines
{
    public class RasterRenderEngine: RenderingEngine
    {
        // Buffer, to which the scene is rendered
        int framebuffer = 0;

        // Scene color texture
        Texture sceneColorTexture = null;
        
        // Scene depth texture
        Texture sceneDepthTexture = null;

        // Plane object, to which the scene is rendered
        MeshObject renderPlane = null;


        // Post processing
        string postProcessApplicationShaderName = "";

        // TO DO: Post Prcocessing stack of compute shaders


        public RasterRenderEngine(Engine inEngine): base(inEngine) { }


        // Called on engine startup
        public override void SetUp() 
        {
            // Enabling depth testing for correct rendering order
            GL.Enable(EnableCap.DepthTest);

            // Enabling blending for tranparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            // Creating a frame buffer
            framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            // Binding scene color texture to framebuffer (scene will be rendered here)
            sceneColorTexture = new Texture(engine.windowWidth, engine.windowHeight);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, sceneColorTexture.textureHandle, 0);

            // Binding scene depth texture to framebuffer
            sceneDepthTexture = new Texture(engine.windowWidth, engine.windowHeight, TextureType.DepthMap);
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
            engine.AddTexture("ENGINE_SceneColor_T", sceneColorTexture);
            engine.AddTexture("ENGINE_SceneDepth_T", sceneDepthTexture);


            // Default post process shader application
            if (postProcessApplicationShaderName == "")
            {
                string vertexShaderFilePath = engine.EngineFilesDirectory + "Rendering/Shaders/DefaultShaders/PostProcessing/Default/DefaultPostProcessingShader.vert";
                string fragmentShaderFilePath = engine.EngineFilesDirectory + "Rendering/Shaders/DefaultShaders/PostProcessing/Default/DefaultPostProcessingShader.frag";

                engine.AddShader("ENGINE_PostProcessAppl_S", new RenderShader(engine, vertexShaderFilePath, fragmentShaderFilePath));

                postProcessApplicationShaderName = "ENGINE_PostProcessAppl_S";
            }


            // Initiating render plane
            engine.AddMeshData("ENGINE_RenderPlane_M", new RenderPlaneMesh());

            renderPlane = new MeshObject(engine, "ENGINE_RenderPlane_M", postProcessApplicationShaderName, new string[] { "ENGINE_SceneColor_T", "ENGINE_SceneDepth_T" });
        }


        // Called everyframe to render the scene
        public override void Render() 
        {
            // Binding frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            // Clearing old stuff in the frame buffer
            GL.ClearColor(0.0f, 0.0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (engine.currentWorld == null) return;

            // Engine-level shader uniforms
            foreach (var shaderPair in engine.shaders)
            {
                var shader = shaderPair.Value;

                shader.UseShader();

                shader.SetUniform("time", engine.currentWorld.time);

                shader.SetUniform("light_direction", engine.currentWorld.lightDirection);
                shader.SetUniform("ambient_light", 0.5f);

                shader.SetUniform("screen_width", (float)engine.windowWidth);
                shader.SetUniform("screen_height", (float)engine.windowHeight);

                shader.SetUniform("camera_location", engine.currentWorld.worldCamera.location);
                shader.SetUniform("camera_vector", engine.currentWorld.worldCamera.forwardVector);

                shader.SetUniform("view", engine.currentWorld.worldCamera.GetViewMatrix(), true);
                shader.SetUniform("projection", engine.currentWorld.worldCamera.GetProjectionMatrix(), true);

                shader.SetUniform("view_inverse", engine.currentWorld.worldCamera.GetViewMatrix().Inverted(), true);
                shader.SetUniform("projection_inverse", engine.currentWorld.worldCamera.GetProjectionMatrix().Inverted(), true);

                shader.StopUsingShader();
            }


            // RENDERING WORLD

            World world = engine.currentWorld;

            // Separating objects into opaque and transparent

            List<MeshObject> opaque = new List<MeshObject>();
            List<(MeshObject obj, float distance)> transparent = new List<(MeshObject, float)>();

            foreach (GameObject obj in world.objects)
            {
                if (!(obj is MeshObject)) continue;

                MeshObject mesh = (MeshObject)obj;
                if (mesh.IsTranparent) transparent.Add((mesh, (mesh.location - world.worldCamera.location).LengthSquared));
                else opaque.Add(mesh);
            }

            // Sorting transparent by distance
            transparent.Sort((a, b) => b.distance.CompareTo(a.distance));

            // Rendering opaque first
            foreach (var obj in opaque) obj.Render(world.worldCamera);

            // Transparent - second
            foreach (var t in transparent) t.obj.Render(world.worldCamera);
        }
    }
}
