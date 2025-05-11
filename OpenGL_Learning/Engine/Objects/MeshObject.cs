using OpenGL_Learning.Engine.Objects.Player;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenGL_Learning.Engine.Rendering.Shaders;
using OpenGL_Learning.Engine.Rendering.Mesh;


namespace OpenGL_Learning.Engine.Objects
{

    public class MeshObject : GameWorldObject
    {
        // Mesh data
        protected MeshData meshData = null;

        // Textures
        Texture[] textures = new Texture[0];

        // Shader
        Shader shader = null;

        // Transformation matrices
        protected Matrix4 locationMatrix = Matrix4.Identity;
        protected Matrix4 rotationMatrix = Matrix4.Identity;
        protected Matrix4 scaleMatrix = Matrix4.Identity;

        // Whether the transformations have changed since the last shader update
        bool transformationsUpdated = true;

        public bool IsTranparent { get; set; }

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

            // Binding shader and passing matricies to it
            shader.UseShader();

            // Binding textures
            for (int i = 0; i < Math.Min(textures.Length, 16); i++)
            {
                textures[i].UseTexture(TextureUnit.Texture0 + i);
                shader.SetUniform("texture" + i, i);
            }

            shader.SetUniform("model", scaleMatrix * rotationMatrix * locationMatrix, true);

            // Passing other uniforms
            BindShaderUniforms();

            // Rendering mesh
            meshData.Render();
        }

        protected virtual void BindShaderUniforms()
        {
            if (!transformationsUpdated) return;

            transformationsUpdated = false;

            // Object-level uniforms
            shader.SetUniform("object_location", location);
            shader.SetUniform("object_rotation", ref rotationMatrix, false);
            shader.SetUniform("object_scale", scale);
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

            transformationsUpdated = true;
        }
    }
}
