using System;
using System.Collections.Generic;
using Client;
using Environment;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class ItemThrower : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [SerializeField] private UnityEvent onItemThrown;
        
        public void ThrowItem(string message)
        {
            if (message == null) return;

            var itemBagMessage = JsonUtility.FromJson<ObjectMessage>(message);
            if (itemBagMessage is not { type: "ShowItem" }) return;

            var index = int.Parse(itemBagMessage.targetObject);
            CollectibleManager.Instance.RemoveItem(itemBagMessage.targetObject);
            spawner.SpawnAtIndex(index);
            onItemThrown.Invoke();
        }
    }
}
