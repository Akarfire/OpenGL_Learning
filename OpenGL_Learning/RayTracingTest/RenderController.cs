using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Player;
using OpenGL_Learning.Engine.Rendering.RenderEngines;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RayTracingTest
{
    public class RenderController: GameObject, InputInterface
    {
        public RenderController(Engine inEngine) : base(inEngine) { }


        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
        {
            if (keyboardState.IsKeyDown(Keys.R) && !keyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (engine.renderingEngine is RayTracingRenderEngine)
                {
                    ((RayTracingRenderEngine)engine.renderingEngine).RayCount = 15;
                    ((RayTracingRenderEngine)engine.renderingEngine).MaxBounces = 5;

                    ((RayTracingRenderEngine)engine.renderingEngine).OnCameraMoved();
                }
            }

            if (keyboardState.IsKeyDown(Keys.R) && keyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (engine.renderingEngine is RayTracingRenderEngine)
                {
                    ((RayTracingRenderEngine)engine.renderingEngine).RayCount = 1;
                    ((RayTracingRenderEngine)engine.renderingEngine).MaxBounces = 2;

                    ((RayTracingRenderEngine)engine.renderingEngine).OnCameraMoved();
                }
            }
        }
    }
}
