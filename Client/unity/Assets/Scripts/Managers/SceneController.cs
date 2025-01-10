using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class SceneController : MonoBehaviourSingleton<SceneController>
    {
        [SerializeField] private string initialScene;
        private string _activeSceneName;

        protected override void Awake()
        {
            base.Awake();
            LoadSceneAdditively(initialScene);
            _activeSceneName = initialScene;
        }

        // Load a scene additively
        public void LoadSceneAdditively(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                // If the scene is loaded but inactive, activate it
                if (!scene.isDirty)
                {
                    SetActiveScene(sceneName);
                }
                else
                {
                    Debug.LogWarning($"Scene {sceneName} is already loaded but active.");
                }
                return;
            }

            // If the scene is not loaded, load it additively
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed += (operation) =>
            {
                Debug.Log($"Scene {sceneName} loaded successfully.");
            };
        }

        // Unload a scene by deactivating it
        public void UnloadScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene {sceneName} is not currently loaded.");
                return;
            }

            // Deactivate the scene without unloading it
            SceneManager.SetActiveScene(scene); // Set the active scene to one that will remain active
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                rootObject.SetActive(false); // Deactivate all root objects in the scene
            }

            Debug.Log($"Scene {sceneName} deactivated successfully.");
        }

        // Transition between scenes
        public void TransitionToScene(string newSceneName)
        {
            if (!string.IsNullOrEmpty(_activeSceneName))
            {
                UnloadScene(_activeSceneName); // Deactivate the current active scene
            }

            LoadSceneAdditively(newSceneName); // Load the new scene additively
            _activeSceneName = newSceneName; // Update active scene name
        }

        // Set the active scene and reactivate all root objects
        public void SetActiveScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene); // Set the scene as active
                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    rootObject.SetActive(true); // Reactivate all root objects
                }
                Debug.Log($"Active scene set to {sceneName}");
            }
            else
            {
                Debug.LogWarning($"Scene {sceneName} is not valid or loaded.");
            }
        }
    }
}
