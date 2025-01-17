using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Collector : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onCollect;
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collectible = other.GetComponent<Collectible>();
            if (collectible == null)
            {
                return;
            }
            
            collectible.Collect();
            onCollect.Invoke(collectible.CollectibleId);
        }
    }
}