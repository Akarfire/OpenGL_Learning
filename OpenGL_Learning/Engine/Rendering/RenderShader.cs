using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.Rendering
{
    public class RenderShader : Shader
    {
        public RenderShader(Engine inEngine, string vertexShaderFile, string fragmentShaderFile) : base(inEngine)
        {
            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderFile);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderFile);

            // Binding and linking the program

            GL.AttachShader(shaderHandle, vertexShader);
            GL.AttachShader(shaderHandle, fragmentShader);

            GL.LinkProgram(shaderHandle);

            AutoRegisterUniforms();

            // Clean up
            GL.DetachShader(shaderHandle, vertexShader);
            GL.DetachShader(shaderHandle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }
    }
}
