using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Rendering.DefaultMeshData;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using OpenGL_Learning.Engine.Objects.Player;
using OpenGL_Learning.Engine.Rendering.Mesh;
using OpenGL_Learning.Engine.Rendering;


namespace OpenGL_Learning.Engine.Rendering.RenderEngines
{
    public class RayTracingRenderEngine: RenderingEngine
    {
        // Scene color texture
        Texture sceneColorTexture = null;

        // TO DO: Add scene depth rendering for ray tracing
        
        // Scene depth texture
        //Texture sceneDepthTexture = null;

        // Plane object, to which the scene is rendered
        MeshObject renderPlane = null;

        // Post processing
        public string postProcessApplicationShaderName = "";

        // TO DO: Post Processing stack of compute shaders

        // Ray tracing

        public string rayTracingComputeShader = "";

        private int lightsSSBO = 0;
        private int cameraSSBO = 0;
        private int materialSSBO = 0;

        private int objectSSBO = 0;
        private int bvhSSBO = 0;
        private int triangleSSBO = 0;

        // Ray tracing accumulation
        private int frameCountSinceLastCameraMovement = 0;

        // Other
        int lastFrameNumberOfLights;
        bool refreshMeshSSBO = false;

        ObjectData[] cachedObjectData = null;
        BVHNode[] cachedBVHTree = null;
        RenderTriangle[] cachedTriangles = null;

        // Material storage
        public Dictionary<string, Material> materials { get; private set; } = new Dictionary<string, Material>()
        {
            // Default material
            { "DefaultMaterial", new Material() }
        };


        // RUNTIME PARAMETERS
        public int RayCount { get; set; } = 1;
        public int MaxBounces { get; set; } = 2;

        // ----

        public RayTracingRenderEngine(Engine inEngine) : base(inEngine) { }


        // Called on engine startup
        public override void SetUp()
        {

            sceneColorTexture = new Texture(engine.windowWidth, engine.windowHeight, TextureType.ComputeShaderOutput);
            engine.AddTexture("ENGINE_SceneColor_T", sceneColorTexture);

            // Initiating render plane
            engine.AddMeshData("ENGINE_RenderPlane_M", new RenderPlaneMesh());

            // Registering default shader
            if (postProcessApplicationShaderName == "")
            {
                engine.AddShader("ENGINE_RayTracingRenderPlane_S", new RenderShader(engine,
                    engine.EngineFilesDirectory + "Rendering/Shaders/DefaultShaders/RayTracingRenderPlane/DefaultRayTracingRenderPlaneShader.vert",
                    engine.EngineFilesDirectory + "Rendering/Shaders/DefaultShaders/RayTracingRenderPlane/DefaultRayTracingRenderPlaneShader.frag"));

                postProcessApplicationShaderName = "ENGINE_RayTracingRenderPlane_S";
            }

            renderPlane = new MeshObject(engine, "ENGINE_RenderPlane_M", postProcessApplicationShaderName, new string[] { "ENGINE_SceneColor_T" });


            // Generating BVHs for mesh data
            foreach (var mesh in engine.meshes)
            {
                if (mesh.Value is RayTracingMeshData)
                    ((RayTracingMeshData)(mesh.Value)).GenerateBVH();
            }


            // Generating SSBOs

            // Camera SSBO
            cameraSSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, cameraSSBO);

            CameraData initialCameraData = new CameraData();
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf<CameraData>(), ref initialCameraData, BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, cameraSSBO); // Binding camera SSBO to "slot" 0

            // Lights SSBO
            lightsSSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, lightsSSBO);

            List<LightData> initialLightData = new List<LightData>();
            GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf<LightData>() * initialLightData.Count, initialLightData.ToArray(), BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, lightsSSBO); // Binding light SSBO to "slot" 1

            // Materials SSBO
            materialSSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, materialSSBO);

            Material[] materialsData = materials.Values.ToArray();
            GL.BufferData(BufferTarget.ShaderStorageBuffer, materialsData.Length * Marshal.SizeOf<Material>(), materialsData, BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, materialSSBO);


            // MESH SSBOs
            FetchMeshDataFromWorld();

            // Object SSBO
            objectSSBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, objectSSBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, cachedObjectData.Length * Marshal.SizeOf<ObjectData>(), cachedObjectData, BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 3, objectSSBO);


            // BVH SSBO
            bvhSSBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, bvhSSBO);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, cachedBVHTree.Length * Marshal.SizeOf<BVHNode>(), cachedBVHTree, BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, bvhSSBO);


            // Triangles SSBO
            triangleSSBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, triangleSSBO);

            GL.BufferData(BufferTarget.ShaderStorageBuffer, cachedTriangles.Length * Marshal.SizeOf<RenderTriangle>(), cachedTriangles, BufferUsageHint.DynamicDraw);

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 5, triangleSSBO);



            // Error checking
            if (!engine.shaders.ContainsKey(rayTracingComputeShader)) throw new Exception("ERROR: No ray tracing compute shader specified!");
        }


        // Called everyframe to render the scene
        public override void Render(float deltaTime)
        {

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


            // Ray tracing
            ComputeShader rayTracingShader = (ComputeShader)engine.shaders[rayTracingComputeShader];

            // Accumulation logic
            frameCountSinceLastCameraMovement++;

            // Sending data to SSBOs

            // Camera SSBO
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, cameraSSBO);
            CameraData cameraData = GetCurrentCameraData();
            GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, Marshal.SizeOf<CameraData>(), ref cameraData);

            // Lights SSBO
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, lightsSSBO);

            List<LightData> lightData = engine.currentWorld.GetLightData();

            /* If number the number of lights has changed since the last frame, then we have to realocate buffer's memory;
             * If it didn't change: we can just upload our data to the already allocated memory
             */
            if (lastFrameNumberOfLights == lightData.Count)
                GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, Marshal.SizeOf<LightData>() * lightData.Count, lightData.ToArray());

            else
                GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf<LightData>() * lightData.Count, lightData.ToArray(), BufferUsageHint.DynamicDraw);

            lastFrameNumberOfLights = lightData.Count;

            // Binding output image
            GL.BindImageTexture(0, engine.textures["ENGINE_SceneColor_T"].textureHandle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);


            rayTracingShader.UseShader();

            // Setting uniforms
            rayTracingShader.SetUniform("lightCount", lightData.Count);
            rayTracingShader.SetUniform("objectCount", cachedObjectData.Length);
            rayTracingShader.SetUniform("bvhCount", cachedBVHTree.Length);
            rayTracingShader.SetUniform("triangleCount", cachedTriangles.Length);

            rayTracingShader.SetUniform("frame", frameCountSinceLastCameraMovement);

            rayTracingShader.SetUniform("rayCount", RayCount);
            rayTracingShader.SetUniform("maxBounces", MaxBounces);

            rayTracingShader.SetUniform("screen_width", (float)engine.windowWidth);
            rayTracingShader.SetUniform("screen_height", (float)engine.windowHeight);


            // Dispatching compute shader
            int groupSizeX = 8;
            int groupSizeY = 8;

            int groupsX = (engine.windowWidth + groupSizeX - 1) / groupSizeX;
            int groupsY = (engine.windowHeight + groupSizeY - 1) / groupSizeY;

            // Dispatching the compute shader
            rayTracingShader.DispatchShader(groupsX, groupsY, MemoryBarrierFlags.ShaderImageAccessBarrierBit);


            // DRAWING RENDER PLANE

            // Back to screen rendering
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Clearing old stuff on screen
            GL.ClearColor(0.0f, 0.0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Rendering render plane to screen
            renderPlane.Render(engine.currentWorld.worldCamera);
        }


        CameraData GetCurrentCameraData()
        {
            CameraData cameraData = new CameraData();

            if (engine.currentWorld == null) return cameraData;
            Camera camera = engine.currentWorld.worldCamera;

            // Setting data
            cameraData.location = camera.location;

            cameraData.forwardVector = camera.forwardVector;
            cameraData.rightVector = camera.rightVector;
            cameraData.upVector = camera.upVector;

            cameraData.fieldOfView = MathHelper.DegreesToRadians(camera.fov);
            cameraData.aspectRatio = (float)engine.windowWidth / engine.windowHeight;

            return cameraData;
        }

        void FetchMeshDataFromWorld()
        {
            List<RenderTriangle> triangles = new List<RenderTriangle>();
            List<ObjectData> objects = new List<ObjectData>();
            List<BVHNode> bvhTree = new List<BVHNode>();

            for(int i = 0; i < engine.currentWorld.objects.Count; i++)
            {
                GameObject obj = engine.currentWorld.objects[i];
                if (obj is RayTracingMeshObject)
                    if ( ((RayTracingMeshObject)obj).meshData is RayTracingMeshData )
                    {

                        // Casting
                        RayTracingMeshObject meshObject = ((RayTracingMeshObject)obj);

                        RayTracingMeshData meshData = (RayTracingMeshData)(meshObject.meshData);

                        ObjectData objectData = meshObject.GetRayTracingObjectData();


                        // Object data
                        objectData.bvhStart = bvhTree.Count;
                        objectData.trianglesStart = triangles.Count;
                        //.bvhStart = meshData.BVH_triangles.Count;

                        objects.Add(objectData);


                        // BVH

                        Matrix4 model = meshObject.GetModelMatrix();

                        // Aplying model matrix to the BVH node extents
                        List<BVHNode> bvhNodes = meshData.BVH_tree;
                        
                        for(int j = 0; j < bvhNodes.Count; j++)
                        {
                            BVHNode node = bvhNodes[j];

                            node.minExtent = (new Vector4(node.minExtent, 1) * model).Xyz;
                            node.maxExtent = (new Vector4(node.maxExtent, 1) * model).Xyz;

                            bvhNodes[j] = node;
                        }

                        bvhTree.AddRange(bvhNodes);


                        // Triangles

                        // Applying model matrix to the triangles
                        List<RenderTriangle> tris = meshData.BVH_triangles;
                        
                        for (int j = 0; j < tris.Count; j++)
                        {
                            RenderTriangle triangle = tris[j];

                            triangle.v1 = (new Vector4(triangle.v1, 1) * model).Xyz;
                            triangle.v2 = (new Vector4(triangle.v2, 1) * model).Xyz;
                            triangle.v3 = (new Vector4(triangle.v3, 1) * model).Xyz;

                            Vector3 normal = new Vector3(triangle.normalX, triangle.normalY, triangle.normalZ);
                            normal = (new Vector4(normal, 0) * model).Xyz;

                            triangle.normalX = normal.X;
                            triangle.normalY = normal.Y;
                            triangle.normalZ = normal.Z;

                            tris[j] = triangle;
                        }

                        triangles.AddRange(tris);
                    }
            }

            cachedBVHTree = bvhTree.ToArray();
            cachedObjectData = objects.ToArray();
            cachedTriangles = triangles.ToArray();
        }


        public override void OnCameraMoved()
        {
            base.OnCameraMoved();
            frameCountSinceLastCameraMovement = 0;
        }


        // Material management

        // Registers new material in the rendering engine
        public void AddMaterial(string materialName, Material material) { materials.Add(materialName, material); }

        // Removes mesh data from the engine's registry
        public void RemoveMeshData(string materialName)
        {
            if (!materials.ContainsKey(materialName)) return;

            materials.Remove(materialName);
        }
    }
}
