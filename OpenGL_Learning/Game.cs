using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace OpenGL_Learning
{
    internal class Game: GameWindow
    {
        int width = 1280, height = 720;

        //float[] triangle_vertices =
        //{
        //    0f,     0.5f,   0f,
        //    -0.5f,  -0.5f,  0f,
        //    0.5f,   -0.5f,  0f
        //};

        float[] square_vertices =
        {
            -0.5f,  0.5f,   0f, // top left vertex - 0 
             0.5f,  0.5f,   0f, // top right vertex - 1 
             0.5f, -0.5f,   0f, // bottom right vertex - 2
            -0.5f, -0.5f,   0f
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        float[] texCoords =
        {
            0f, 1f,
            1f, 1f,
            1f, 0f,
            0f, 0f
        };
        int textureVBO;

        int VAO;
        int VBO;
        int EBO;

        //int texture;

        string shaderFolder = "../../../Shaders/";
        string textureFolder = "../../../Textures/";

        Texture texture;
        Shader shader;


        public Game(int width, int height) : base
        (GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(width, height));
            this.height = height;
            this.width = width;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, square_vertices.Length * sizeof(float), square_vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);

            // EBO
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Shaders
            shader = new Shader(shaderFolder + "shader.vert", shaderFolder + "shader.frag");
            shader.UseShader();

            // texture VBO
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);


            // Textures
            texture = new Texture(textureFolder + "Artem_ChibiArt.png");
            texture.UseTexture(TextureUnit.Texture0);
            
        }

        protected override void OnUnload()
        {
            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);

            texture.DeleteTexture();
            shader.DeleteShader();
            
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            texture.UseTexture(TextureUnit.Texture0);

            shader.UseShader();
            GL.Uniform1(GL.GetUniformLocation(shader.GetHandle(), "texture0"), 0);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape)) 
            {
                Close();
            }
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


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, width, height);

            this.width = e.Width;
            this.height = e.Height;
        }

    }
}
