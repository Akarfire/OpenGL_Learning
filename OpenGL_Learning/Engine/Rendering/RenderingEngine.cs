using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Rendering
{
    public abstract class RenderingEngine
    {
        public Engine engine { get; private set; }

        public RenderingEngine(Engine inEngine) { engine = inEngine; }


        // Called on engine startup
        public abstract void SetUp();

        // Called everyframe to render the scene
        public abstract void Render();

    }
}
