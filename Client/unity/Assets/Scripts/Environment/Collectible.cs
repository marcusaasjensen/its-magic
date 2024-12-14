using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private ParticleSystem collectibleParticles;
        [SerializeField] protected UnityEvent onCollect;
        [SerializeField] private string collectorName = "Player";
        private string _collectibleId;

        private void Start() => OnCollectible();
        
        private void OnCollectible()
        {
            _collectibleId = CollectibleManager.GenerateCollectibleId(gameObject);

            if (!CollectibleManager.IsItemCollected(_collectibleId))
            {
                return;
            }
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(collectorName))
            {
                return;
            }
            Collect();
        }

        private void PlayParticles()
        {
            if (collectibleParticles == null)
            {
                return;
            }
            collectibleParticles.transform.position = transform.position;
            collectibleParticles.Play();
        }
        
        // protected abstract void WhenCollected();

        public void Collect()
        {
            // WhenCollected();
            CollectibleManager.Instance.CollectItem(_collectibleId);
            PlayParticles();
            onCollect.Invoke();
            Destroy(gameObject);
        }
    }
}