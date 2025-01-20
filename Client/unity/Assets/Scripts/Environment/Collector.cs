using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Collector : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onCollect;
        [SerializeField] private bool sendItemToServer = true;
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collectible = other.GetComponent<Collectible>();
            if (collectible == null)
            {
                return;
            }
            
            collectible.Collect(sendItemToServer);
            onCollect.Invoke(collectible.CollectibleId);
        }
    }
}