using System;
using Data;
using UnityEngine;
using Utils;

namespace Managers
{
    public class CollectibleManager : SceneSingleton<CollectibleManager>
    {
        [SerializeField] public GlobalData globalData;
        [SerializeField] private bool save;

        public event Action OnItemCollected;

        public static string GenerateCollectibleId(GameObject o)
        {
            var id = o.name + o.transform.position;
            return id;
        }
        
        public static bool IsItemCollected(string id) => Instance.globalData.collectedItemKeys.Contains(id);
        
        public void CollectItem(string id)
        {
            if (IsItemCollected(id))
            {
                return;
            }

            if (save)
            {
                globalData.collectedItemKeys.Add(id);
            }

            OnItemCollected?.Invoke();
        }
    }
}

