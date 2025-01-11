using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SceneState", menuName = "Data/SceneState")]
    public class SceneState : ScriptableObject
    {
        public string sceneName;

        private Dictionary<string, object> _customData = new ();

        public void SetData<T>(string key, T value)
        {
            if (_customData.ContainsKey(key))
            {
                _customData[key] = value;
            }
            else
            {
                _customData.Add(key, value);
            }
        }

        public T GetData<T>(string key, T defaultValue = default)
        {
            if (_customData.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
 
        public bool HasKey(string key)
        {
            return _customData.ContainsKey(key);
        }

        public void PrintAllData()
        {
            foreach (var entry in _customData)
            {
                Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
            }
        }

        public void RemoveData(string key)
        {
            if (_customData.ContainsKey(key))
            {
                _customData.Remove(key);
            }
        }

        public void RemoveAllData()
        {
            _customData.Clear();
        }
    }
}