using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public abstract class Collectible : MonoBehaviour
    {
        [SerializeField] private bool saveOnCollected;
        [SerializeField] private ParticleSystem collectibleParticles;
        [SerializeField] protected UnityEvent onCollect;
        private string _collectibleId;

        private void Start() => OnCollectible();
        
        private void OnCollectible()
        {
            _collectibleId = CollectibleManager.GenerateCollectableId(gameObject);

            if (!CollectibleManager.IsItemCollected(_collectibleId) || !saveOnCollected)
            {
                return;
            }
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Collect();
            }
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
        
        protected abstract void WhenCollected();

        private void Collect()
        {
            WhenCollected();
            CollectibleManager.Instance.CollectItem(_collectibleId, saveOnCollected);
            PlayParticles();
            onCollect.Invoke();
            Destroy(gameObject);
        }
    }
}