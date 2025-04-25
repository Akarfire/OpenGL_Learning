using OpenGL_Learning.Engine.Objects.Player;
using OpenGL_Learning.Engine.Rendering;
using OpenTK.Mathematics;


namespace OpenGL_Learning.Engine.Objects
{

    public class MeshObject : GameWorldObject
    {
        // Mesh data
        protected MeshData meshData = null;

        // Textures
        Texture[] textures = null;

        // Shader
        Shader shader = null;

        // Transformation matrices
        protected Matrix4 locationMatrix { get; set; } = Matrix4.Identity;
        protected Matrix4 rotationMatrix { get; set; } = Matrix4.Identity;
        protected Matrix4 scaleMatrix { get; set; } = Matrix4.Identity;

        public MeshObject(Engine inEngine, string meshDataName = null, string shaderHandle = null, string[] textureHandles = null) : base(inEngine)
        {
            if (meshDataName != null)
                meshData = engine.meshes[meshDataName];

            if (shaderHandle != null)
                shader = engine.shaders[shaderHandle];

            if (textureHandles != null)
            {
                textures = new Texture[textureHandles.Length];
                for (int i = 0; i < textureHandles.Length; i++) { textures[i] = engine.textures[textureHandles[i]]; }
            }
        }

        public void SetShader(string shaderName)
        {
            shader = engine.shaders[shaderName];
        }

        public void SetTextures(string[] textureNames)
        {
            textures = new Texture[textureNames.Length];

            for (int i = 0; i < textureNames.Length; i++) { textures[i] = engine.textures[textureNames[i]]; }
        }

        public void Render(Camera camera)
        {
            // Receiving matricies from the camera
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            // Binding textures
            textures[0].UseTexture(0);

            // Binding shader and passing matricies to it
            shader.UseShader();
            shader.bindMatrices(scaleMatrix * rotationMatrix * locationMatrix, view, projection);

            meshData.Render();
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();
        }

        protected override void OnTransformationUpdated()
        {
            base.OnTransformationUpdated();

            // Updating transformation matricies
            locationMatrix = Matrix4.CreateTranslation(location);
            rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-1 *rotation.Y)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            scaleMatrix = Matrix4.CreateScale(scale);
        }
    }
}
