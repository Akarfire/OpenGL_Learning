using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine;
using OpenGL_Learning.Engine.Player;
using OpenGL_Learning.Engine.Rendering.RenderEngines;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RayTracingTest
{
    public class RenderController: GameObject, InputInterface
    {
        public RenderController(Engine inEngine) : base(inEngine)
        {
            previewMode();
        }


        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
        {
            if (keyboardState.IsKeyDown(Keys.R) && !keyboardState.IsKeyDown(Keys.LeftControl)) renderMode();

            if (keyboardState.IsKeyDown(Keys.R) && keyboardState.IsKeyDown(Keys.LeftControl)) previewMode();
        }


        private void previewMode()
        {
            if (engine.renderingEngine is RayTracingRenderEngine)
            {
                ((RayTracingRenderEngine)engine.renderingEngine).RayCount = 1;
                ((RayTracingRenderEngine)engine.renderingEngine).MaxBounces = 4;

                ((RayTracingRenderEngine)engine.renderingEngine).OnCameraMoved();
            }
        }

        private void renderMode()
        {
            if (engine.renderingEngine is RayTracingRenderEngine)
            {
                ((RayTracingRenderEngine)engine.renderingEngine).RayCount = 5;
                ((RayTracingRenderEngine)engine.renderingEngine).MaxBounces = 8;

                ((RayTracingRenderEngine)engine.renderingEngine).OnCameraMoved();
            }
        }
    }
}
