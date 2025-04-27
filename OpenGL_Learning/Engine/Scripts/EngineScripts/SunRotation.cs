using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Scripts.EngineScripts
{
    internal class SunRotation: Script
    {
        public SunRotation() { }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (owner != null) 
            {
                owner.engine.currentWorld.lightDirection = Matrix3.CreateRotationY(0.05f * deltaTime) * owner.engine.currentWorld.lightDirection;
            }
        }
    }
}
