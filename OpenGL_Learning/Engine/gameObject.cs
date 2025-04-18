using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine
{
    public class GameObject
    {
        public Engine engine { get; private set; }
        public GameObject(Engine inEngine) { engine = inEngine; }


        // Called when the object is spawned into the world
        public virtual void onSpawned() { }

        // Called when the object is about to be destroyed
        public virtual void onDestroyed() { }

        // Called every game frame
        public virtual void onUpdated(float deltaTime) { }

    }
}
