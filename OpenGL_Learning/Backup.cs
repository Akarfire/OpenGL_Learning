/*
 using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        int width = 800, height = 800;

        //float[] triangle_vertices =
        //{
        //    0f,     0.5f,   0f,
        //    -0.5f,  -0.5f,  0f,
        //    0.5f,   -0.5f,  0f
        //};

        //float[] square_vertices =
        //{
        //    -0.5f,  0.5f,   0f, 
        //     0.5f,  0.5f,   0f, 
        //     0.5f, -0.5f,   0f, 
        //    -0.5f, -0.5f,   0f
        //};

        List<Vector3> vertices = new List<Vector3>()
        {
            new Vector3( -0.5f,  0.5f,   0.5f),
            new Vector3(  0.5f,  0.5f,   0.5f),
            new Vector3(  0.5f, -0.5f,   0.5f),
            new Vector3( -0.5f, -0.5f,   0.5f),

            new Vector3( -0.5f,  0.5f,   -0.5f),
            new Vector3(  0.5f,  0.5f,   -0.5f),
            new Vector3(  0.5f, -0.5f,   -0.5f),
            new Vector3( -0.5f, -0.5f,   -0.5f)
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            1, 5, 6,
            6, 2, 1,

            0, 4, 7,
            7, 3, 0,

            0, 4, 5,
            5, 1, 0,

            3, 2, 6,
            6, 7, 3
        };

        List<Vector2> texCoords = new List<Vector2>()
        {
            // Front face (0, 1, 2, 3)
            new Vector2(0.0f, 0.0f), // Vertex 0
            new Vector2(1.0f, 0.0f), // Vertex 1
            new Vector2(1.0f, 1.0f), // Vertex 2
            new Vector2(0.0f, 1.0f), // Vertex 3

            // Back face (4, 5, 6, 7)
            new Vector2(1.0f, 0.0f), // Vertex 4
            new Vector2(0.0f, 0.0f), // Vertex 5
            new Vector2(0.0f, 1.0f), // Vertex 6
            new Vector2(1.0f, 1.0f), // Vertex 7

            // Top face (0, 1, 5, 4)
            new Vector2(0.0f, 1.0f), // Vertex 8
            new Vector2(1.0f, 1.0f), // Vertex 9
            new Vector2(1.0f, 0.0f), // Vertex 10
            new Vector2(0.0f, 0.0f), // Vertex 11

            // Bottom face (3, 2, 6, 7)
            new Vector2(0.0f, 0.0f), // Vertex 12
            new Vector2(1.0f, 0.0f), // Vertex 13
            new Vector2(1.0f, 1.0f), // Vertex 14
            new Vector2(0.0f, 1.0f), // Vertex 15

            // Left face (0, 3, 7, 4)
            new Vector2(0.0f, 0.0f), // Vertex 16
            new Vector2(1.0f, 0.0f), // Vertex 17
            new Vector2(1.0f, 1.0f), // Vertex 18
            new Vector2(0.0f, 1.0f), // Vertex 19

            // Right face (1, 2, 6, 5)
            new Vector2(0.0f, 0.0f), // Vertex 20
            new Vector2(1.0f, 0.0f), // Vertex 21
            new Vector2(1.0f, 1.0f), // Vertex 22
            new Vector2(0.0f, 1.0f), // Vertex 23
        };

        //float[] texCoords =
        //{
        //    0f, 1f,
        //    1f, 1f,
        //    1f, 0f,
        //    0f, 0f
        //};
        int textureVBO;

        int VAO;
        int VBO;
        int EBO;

        //int texture;

        string shaderFolder = "../../../Shaders/";
        string textureFolder = "../../../Textures/";

        private meshObject[] objects;

        Texture texture;
        Shader shader;

        float time = 0;

        Camera camera;
        bool cursorGrabbed = true;

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
            EBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);

            // EBO
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Shaders
            shader = new Shader(shaderFolder + "shader.vert", shaderFolder + "shader.frag");
            shader.UseShader();

            // texture VBO
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);


            // Textures
            texture = new Texture(textureFolder + "wood.jpg");
            texture.UseTexture(TextureUnit.Texture0);

            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
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
            time += (float)args.Time;

            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            texture.UseTexture(TextureUnit.Texture0);

            shader.UseShader();
            GL.Uniform1(GL.GetUniformLocation(shader.GetHandle(), "texture0"), 0);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            model = Matrix4.CreateRotationY(time * 0.25f) * Matrix4.CreateRotationZ(time * 0.25f);
            Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);

            model *= translation;

            int modelLocation = GL.GetUniformLocation(shader.GetHandle(), "model");
            int viewLocation = GL.GetUniformLocation(shader.GetHandle(), "view");
            int projectionLocation = GL.GetUniformLocation(shader.GetHandle(), "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);


            GL.DrawElements(PrimitiveType.Triangles, indices.Length * sizeof(uint), DrawElementsType.UnsignedInt, 0);


            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            camera.Update(KeyboardState, MouseState, args);

            if (KeyboardState.IsKeyDown(Keys.Escape)) 
            {
                Close();
            }

            if (KeyboardState.IsKeyReleased(Keys.F1) && KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (cursorGrabbed) CursorState = CursorState.Normal;
                else CursorState = CursorState.Grabbed;

                cursorGrabbed = !cursorGrabbed;
                camera.SetMouseInputEnabled(cursorGrabbed);
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

            this.width = e.Width;
            this.height = e.Height;

            if (camera != null) camera.UpdateWindowSize(width, height);

            GL.Viewport(0, 0, width, height);
        }

    }
}

 */

/*
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OpenGL_Learning.meshObjects
{
    internal class GridObject: MeshObject
    {
        protected int sizeX, sizeZ;
        protected float cellSize;
        public GridObject(int inSizeX, int inSizeZ, float inCellSize, Game inGame, Shader inShader, Texture[] inTextures)
        {
            sizeX = inSizeX;
            sizeZ = inSizeZ;
            cellSize = inCellSize;

            vertices = new List<Vector3>();
            indices = new List<uint>();
            texCoords = new List<Vector2>();

            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeZ; z++)
                {
                    ///*
                        new Vector3( -0.5f,  0.5f,   0f),
                        new Vector3(  0.5f,  0.5f,   0f),
                        new Vector3(  0.5f, -0.5f,   0f),
                        new Vector3( -0.5f, -0.5f,   0f),
                     //

using OpenGL_Learning;
using System.Runtime.CompilerServices;

uint oVert = (uint)vertices.Count;
vertices.Add(new Vector3((-0.5f + x) * cellSize, 0f, (0.5f + z) * cellSize));
vertices.Add(new Vector3((0.5f + x) * cellSize, 0f, (0.5f + z) * cellSize));
vertices.Add(new Vector3((0.5f + x) * cellSize, 0f, (-0.5f + z) * cellSize));
vertices.Add(new Vector3((-0.5f + x) * cellSize, 0f, (-0.5f + z) * cellSize));

///*
    0, 1, 2,
    2, 3, 0


indices.Add(oVert);
indices.Add(oVert + 1);
indices.Add(oVert + 2);
indices.Add(oVert + 2);
indices.Add(oVert + 3);
indices.Add(oVert);

/*
    new Vector2(0.0f, 0.0f),
    new Vector2(1.0f, 0.0f),
    new Vector2(1.0f, 1.0f),
    new Vector2(0.0f, 1.0f)

texCoords.Add(new Vector2(0.0f, 0.0f));
texCoords.Add(new Vector2(1.0f, 0.0f));
texCoords.Add(new Vector2(1.0f, 1.0f));
texCoords.Add(new Vector2(0.0f, 1.0f));
                }

            InitMeshObject(inGame, inShader, inTextures);
        }
    }
}

 */