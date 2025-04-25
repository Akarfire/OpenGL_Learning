using OpenGL_Learning.Engine.Objects;
using OpenGL_Learning.Engine.Objects.Player;
using OpenGL_Learning.Engine.Player;

namespace OpenGL_Learning.Engine
{
    public class World
    {
        // Engine
        public Engine engine {  get; private set; }

        // Object list
        protected List<GameObject> objects { get; set; } = new List<GameObject>();


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
            worldCamera.SetMouseInputEnabled(engine.cursorGrabbed);
        }

        public void OnDestroy() { }

        public void Update(float deltaTime) 
        {
            time += deltaTime;

            foreach (GameObject obj in objects) 
            {
                obj.OnUpdated(deltaTime);

                if (obj is InputInterface) ((InputInterface)obj).onUpdateInput(deltaTime, engine.cachedKeyboardState, engine.cachedMouseState);
            }
        }

        public void RenderWorld(float deltaTime) 
        {
            foreach (GameObject obj in objects)
            {
                if (obj is MeshObject) ((MeshObject)obj).Render(worldCamera);
            }
        }


        // Object management
        public void AddObject(GameObject obj) 
        { 
            objects.Add(obj); 
            obj.OnSpawned();
        }

        public void RemoveObject(GameObject obj)
        {
            obj.OnDestroyed();
            objects.Remove(obj);
        }
    }
}
