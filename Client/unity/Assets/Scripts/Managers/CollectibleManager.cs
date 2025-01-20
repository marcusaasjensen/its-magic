using System;
using Data;
using UnityEngine;
using Utils;

namespace Managers
{
    public class CollectibleManager : MonoBehaviourSingleton<CollectibleManager>
    {
        [SerializeField] public GlobalData globalData;

        public event Action OnItemCollected;

        public static string GenerateCollectibleId(GameObject o)
        {
            var id = o.name + o.transform.position;
            return id;
        }
        
        public void CollectItem(string id)
        {
            globalData.collectedItemKeys.Add(id);
            OnItemCollected?.Invoke();
        }
        
        public void RemoveItem(string id)
        {
            globalData.collectedItemKeys.Remove(id);
        }
    }
}

