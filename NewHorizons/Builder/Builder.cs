using NewHorizons.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NewHorizons.Builder
{
    public abstract class Builder<T> : MonoBehaviour where T : Module
    {
        public abstract bool WorksAtEye { get; }
        public virtual (GameObject gameObject, Component component) Make(GameObject root, Sector sector, T module) { return (null, null); }
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
