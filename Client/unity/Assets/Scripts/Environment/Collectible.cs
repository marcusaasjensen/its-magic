using Client;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private string collectibleId = "1";
        [SerializeField] private ParticleSystem collectibleParticles;
        [SerializeField] protected UnityEvent onCollect;
        [SerializeField] private string collectorName = "Player";

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

        private void WhenCollected()
        {
            var addItem = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = collectibleId
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addItem));
        }

        public void Collect()
        {
            WhenCollected();
            CollectibleManager.Instance.CollectItem(collectibleId);
            PlayParticles();
            onCollect.Invoke();
            Destroy(gameObject);
        }
    }
}