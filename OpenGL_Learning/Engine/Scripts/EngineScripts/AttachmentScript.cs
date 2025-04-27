using OpenGL_Learning.Engine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.Scripts.EngineScripts
{
    public class AttachmentScript: Script
    {
        GameWorldObject ownerGW = null;
        public GameWorldObject attachementParent = null;

        // ----

        public AttachmentScript() { }

        protected override void OnScriptAttached()
        {
            base.OnScriptAttached();

            if (owner is GameWorldObject) ownerGW = (GameWorldObject)owner;
        }

        protected override void OnScriptUpdated(float deltaTime)
        {
            base.OnScriptUpdated(deltaTime);

            if (ownerGW != null && attachementParent != null) 
            {
                ownerGW.SetLocation(attachementParent.location);
            }
        }
    }
}
