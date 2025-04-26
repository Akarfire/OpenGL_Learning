using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

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
        }

        protected override void OnUnload()
        {
            engine.ShutdownEngine();
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            engine.Render((float)args.Time);

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            engine.CacheInput(KeyboardState, MouseState);
            engine.Update((float)args.Time);

            base.OnUpdateFrame(args);
        }


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            engine.OnWindowResized(e.Width, e.Height);
        }
    }
}
