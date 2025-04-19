using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine.objects
{
    public class GameObject
    {
        public Engine engine { get; private set; }

        public Dictionary<string, Script> scripts { get; private set; } = new Dictionary<string, Script>();


        // ----

        public GameObject(Engine inEngine) { engine = inEngine; }


        // Called when the object is spawned into the world
        public virtual void OnSpawned() { }

        // Called when the object is about to be destroyed
        public virtual void OnDestroyed()
        {
            foreach (var script in scripts) { script.Value.DestroyScript(); }
        }

        // Called every game frame
        public virtual void OnUpdated(float deltaTime)
        {
            foreach (var script in scripts) { script.Value.UpdateScript(deltaTime); }
        }


        // Script management

        public void AddScript(string scriptName, Script script)
        {
            if (scripts.ContainsKey(scriptName)) { Console.WriteLine("Error: script with name '" + scriptName + "' already exists!"); return; }

            scripts.Add(scriptName, script);
            script.AttachScript(this);
        }

        public void DestroyScript(string scriptName)
        {
            if (!scripts.ContainsKey(scriptName)) { Console.WriteLine("Error: no script with name '" + scriptName, "'!"); return; }

            scripts[scriptName].DestroyScript();
            scripts.Remove(scriptName);
        }
    }
}
