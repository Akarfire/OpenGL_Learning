using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_Learning.Engine.Objects;

namespace OpenGL_Learning.Engine
{
    public class Script
    {
        public GameObject owner { get; private set; } = null;

        // -----

        public Script() { }

        public void AttachScript(GameObject newOwner) 
        {
            if (newOwner == null) return;

            owner = newOwner;
            OnScriptAttached();
        }

        public void DestroyScript() 
        { 
            if (owner != null) { OnScriptDestroyed(); }
        }
        public void UpdateScript(float deltaTime) 
        {
            if (owner != null) { OnScriptUpdated(deltaTime); }
        }


        // To be oberriden in the child classes

        // Called once the script is attached to an owner game object
        protected virtual void OnScriptAttached() { }
        // Called when the owner game object "decided" to destroy this script 
        protected virtual void OnScriptDestroyed() { }
        // Called every update of the owner game object
        protected virtual void OnScriptUpdated(float deltaTime) { }
    }
}
