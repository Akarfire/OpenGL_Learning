using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine.meshObjects;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace OpenGL_Learning.Engine
{
    internal class GameEngineWindow : GameWindow
    {
        
        Engine engine;

        // --------

        public GameEngineWindow(Engine inEngine) : base (GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            engine = inEngine;
            CenterWindow(new Vector2i(engine.windowWidth, engine.windowHeight));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Shaders
            

            // Textures
            textures.Add(new Texture(textureFolder + "wood.jpg"));
            textures.Add(new Texture(textureFolder + "a.png"));
            textures.Add(new Texture(textureFolder + "sea-water-512x512.png"));

            


            camera = new Camera(width, height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            foreach (var item in objects) item.Unload();
            foreach (var item in shaders) item.DeleteShader();
            foreach (var item in textures) item.DeleteTexture();

            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            time += (float)args.Time;

            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            foreach (var obj in objects)
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

            width = e.Width;
            height = e.Height;

            if (camera != null) camera.UpdateWindowSize(width, height);

            GL.Viewport(0, 0, width, height);
        }

    }
}
