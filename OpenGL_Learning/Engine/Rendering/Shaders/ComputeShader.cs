using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGL_Learning.Engine.Rendering.Shaders
{
    public class ComputeShader : Shader
    {
        public ComputeShader(Engine inEngine, string computeShaderFile) : base(inEngine)
        {
            int computeShader = CompileShader(ShaderType.ComputeShader, computeShaderFile);

            // Binding and linking the program
            GL.AttachShader(shaderHandle, computeShader);
            GL.LinkProgram(shaderHandle);

            AutoRegisterUniforms();

            // Clean up
            GL.DetachShader(shaderHandle, computeShader);
            GL.DeleteShader(computeShader);
        }


        // Dispatching compute shader

        public void DispatchShader(int numberOfGroups_X, int numberOfGroups_Y, int numberOfGroups_Z, MemoryBarrierFlags memoryBarrier)
        {
            GL.UseProgram(shaderHandle);
            GL.DispatchCompute(numberOfGroups_X, numberOfGroups_Y, numberOfGroups_Z);

            GL.MemoryBarrier(memoryBarrier);
        }

        public void DispatchShader(int numberOfGroups_X, int numberOfGroups_Y, MemoryBarrierFlags memoryBarrier)
        {
            GL.UseProgram(shaderHandle);
            GL.DispatchCompute(numberOfGroups_X, numberOfGroups_Y, 1);

            GL.MemoryBarrier(memoryBarrier);
        }
    }
}
