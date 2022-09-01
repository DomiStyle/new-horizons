using NewHorizons.External;
using NewHorizons.External.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NewHorizons.Builder
{
    public abstract class AbstractBuilder<T> : MonoBehaviour where T : Module
    {
        public abstract bool WorksAtEye { get; }
        public virtual GameObject Make(GameObject root, Sector sector, T module, BaseModule baseModule) { return null; }
        public virtual void OnSceneLoaded(Scene scene) { }
        public virtual void OnSceneUnloaded(Scene scene) { }

        private void Awake()
        {
            SceneManager.sceneLoaded += (scene, _) => OnSceneLoaded(scene);
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= (scene, _) => OnSceneLoaded(scene);
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }
}
