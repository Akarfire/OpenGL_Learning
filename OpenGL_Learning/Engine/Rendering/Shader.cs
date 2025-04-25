using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace OpenGL_Learning.Engine
{
    public class Shader
    {
        Engine engine;
        int shaderHandle;

        public Shader(Engine inEngine, string vertexShaderFile, string fragmentShaderFile)
        {
            engine = inEngine;
            shaderHandle = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderFile));
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
            if (success1 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(infoLog);
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderFile));
            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(infoLog);
            }

            // Binding

            GL.AttachShader(shaderHandle, vertexShader);
            GL.AttachShader(shaderHandle, fragmentShader);

            GL.LinkProgram(shaderHandle);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public int GetHandle() { return shaderHandle; }

        public void UseShader()
        {
            GL.UseProgram(shaderHandle);

            GL.Uniform1(GL.GetUniformLocation(GetHandle(), "texture0"), 0);

            // Engine level-uniforms
            GL.Uniform1(GL.GetUniformLocation(GetHandle(), "time"), engine.currentWorld.time);
        }

        public void bindMatrices(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            int modelLocation = GL.GetUniformLocation(GetHandle(), "model");
            int viewLocation = GL.GetUniformLocation(GetHandle(), "view");
            int projectionLocation = GL.GetUniformLocation(GetHandle(), "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
        }

        public void DeleteShader()
        {
            GL.DeleteProgram(shaderHandle);
        }

        public static string LoadShaderSource(string filepath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file:" + e.Message);
            }

            return shaderSource;
        }
    }
}
