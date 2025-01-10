using UnityEngine;
using Utils;

namespace Data
{
    public class SceneStateManager : MonoBehaviourSingleton<SceneStateManager>
    {
        [SerializeField] private SceneState sceneState;
        
        public void RemoveData(string key)
        {
            sceneState.RemoveData(key);
        }
        
        public void RemoveAllData()
        {
            sceneState.RemoveAllData();
        }
        
        public void SetData<T>(string key, T value)
        {
            sceneState.SetData(key, value);
        }
        
        public T GetData<T>(string key, T defaultValue = default)
        {
            return sceneState.GetData(key, defaultValue);
        }
        
        public bool HasKey(string key)
        {
            return sceneState.HasKey(key);
        }
    }
}