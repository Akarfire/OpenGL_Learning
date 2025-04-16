using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.meshObjects;
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
        // Screen width / height
        private int width = 1920, height = 1080;

        // Folder paths
        private string shaderFolder = "../../../Shaders/";
        private string textureFolder = "../../../Textures/";

        // Lists of existing elements

        // List of existing shaders
        private List<Shader> shaders = new List<Shader>();
        // List of exisitng textures
        private List<Texture> textures = new List<Texture>();
        // List of existing objects
        private List<MeshObject> objects = new List<MeshObject>();

        // Current time in the game world
        float time = 0;

        // Main camera
        Camera camera;

        // Whether the cursor is grabbed at the moment (if false - the cursor is shown)
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

            // Shaders
            shaders.Add(new Shader(shaderFolder + "shader.vert", shaderFolder + "shader.frag"));
            shaders.Add(new Shader(shaderFolder + "shaderWater.vert", shaderFolder + "shaderWater.frag"));

            // Textures
            textures.Add(new Texture(textureFolder + "wood.jpg"));
            textures.Add(new Texture(textureFolder + "a.png"));
            textures.Add(new Texture(textureFolder + "sea-water-512x512.png"));

            // Objects
            objects.Add(new CubeObject(this, shaders[0], new Texture[] { textures[0] }));
            objects.Add(new PlaneObject(this, shaders[0], new Texture[] { textures[1] }));
            objects.Add(new GridObject(100, 100, 1, this, shaders[1], new Texture[] { textures[2] }));

            objects[1].AddWorldOffset(new Vector3(-1f, 3f, -3f));
            objects[2].AddWorldOffset(new Vector3(0f, -3f, -5f));

            objects[0].SetWorldLocation(new Vector3(1f, 3f, -10f));
            objects[0].SetScale(new Vector3(5f, 5f, 5f));


            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {     
            foreach(var item in objects) item.Unload();
            foreach(var item in shaders) item.DeleteShader();
            foreach(var item in textures) item.DeleteTexture();
            
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            time += (float)args.Time;

            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


           foreach(var obj in objects)
                obj.Render(camera, time);


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
