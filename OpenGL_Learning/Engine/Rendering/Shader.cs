using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace OpenGL_Learning.Engine
{
    public class Shader
    {
        Engine engine;

        public int shaderHandle { get; private set; }

        // Dictionary of all shader uniforms and it's locations
        protected Dictionary<string, int> uniformLocations = new Dictionary<string, int>();


        public Shader(Engine inEngine, string vertexShaderFile, string fragmentShaderFile)
        {
            engine = inEngine;
            shaderHandle = GL.CreateProgram();

            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderFile);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderFile);

            // Binding and linking the program

            GL.AttachShader(shaderHandle, vertexShader);
            GL.AttachShader(shaderHandle, fragmentShader);

            GL.LinkProgram(shaderHandle);

            // Automatically registering uniforms
            GL.GetProgram(shaderHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            for (int i = 0; i < uniformCount; i++) 
            {
                GL.GetActiveUniform(shaderHandle, i, 256, out _, out _, out _, out string uniformName);
                int location = GL.GetUniformLocation(shaderHandle, uniformName);
                uniformLocations[uniformName] = location;
            }

            // Clean up
            GL.DetachShader(shaderHandle, vertexShader);
            GL.DetachShader(shaderHandle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        // Loads and compiles a shader of a given type
        protected static int CompileShader(ShaderType type, string filePath) 
        {
            int handle = GL.CreateShader(type);
            GL.ShaderSource(handle, LoadShaderSource(filePath));
            GL.CompileShader(handle);

            // Error check vertex shader
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(handle);
                throw new Exception($"ERROR: Failed to compile shader {filePath}:\n{infoLog}");
            }

            return handle;
        }

        // Loads shader soruce code from file
        protected static string LoadShaderSource(string filepath)
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
                throw new Exception("Failed to load shader source file:" + e.Message);
            }

            return shaderSource;
        }



        public void UseShader()
        {
            GL.UseProgram(shaderHandle);
        }

        public void StopUsingShader()
        {
            GL.UseProgram(0);
        }

        public void DeleteShader()
        {
            GL.DeleteProgram(shaderHandle);
        }



        // Uniform setters

        public void SetUniform(string uniformName, int value) 
        {
            if (uniformLocations.TryGetValue(uniformName, out int location))
                GL.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector2 value)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.Uniform2(location, value);
        }

        public void SetUniform(string name, Vector3 value)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.Uniform3(location, value);
        }

        public void SetUniform(string name, Vector4 value)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.Uniform4(location, value);
        }

        public void SetUniform(string name, Matrix4 value, bool transpose = false)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.UniformMatrix4(location, transpose, ref value);
        }

        public void SetUniform(string name, ref Matrix4 value, bool transpose = false)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                GL.UniformMatrix4(location, transpose, ref value);
        }
    }
}
