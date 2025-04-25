using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Player
{
    internal interface InputInterface
    {
        public void onUpdateInput(float deltaTime, KeyboardState keyboardState, MouseState mouseState);
    }
}
