using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Learning.Engine
{
    public class World
    {
        // Engine
        public Engine engine {  get; private set; }

        // Object list
        public List<GameObject> objects {  get; private set; } = new List<GameObject>();


        // Main camera of the world
        public Camera worldCamera { get; private set; } = null;


        // Current time in the game world
        public float time { get; private set; } = 0;


        // -------


        public World(Engine inEngine) { engine = inEngine; }

        // General

        public void OnLoad() 
        {
            foreach (GameObject obj in objects) 
            { 
                if (obj is Camera)
                {
                    worldCamera = (Camera)obj;
                    break;
                }
            }

            if (worldCamera == null)
            {
                worldCamera = new Camera(engine);
                AddObject(worldCamera);
            }

            worldCamera.UpdateWindowSize(engine.windowWidth, engine.windowHeight);
        }

        public void OnDestroy() { }

        public void Update(float deltaTime) 
        {
            time += deltaTime;

            foreach (GameObject obj in objects) 
            {
                obj.onUpdated(deltaTime);
            }
        }

        public void RenderWorld(float deltaTime) 
        {
            foreach (GameObject obj in objects)
            {
                if (obj is MeshObject) ((MeshObject)obj).Render(worldCamera)
            }
        }

        public void UpdateInput(float deltaTime) { }


        // Object management
        public int AddObject(GameObject obj) {  objects.Add(obj); return objects.Count - 1; }

        public void RemoveObject(int handle) 
        {
            if (objects.Count <= handle) return;

            objects[handle].onDestroyed();
            objects.RemoveAt(handle);
        }
    }
}
