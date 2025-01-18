using Client;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Environment
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private string collectibleId = "1";
        [SerializeField] private ParticleSystem collectibleParticles;
        [SerializeField] protected UnityEvent onCollect;
        [SerializeField] private string collectorName = "Player";
        
        public string CollectibleId => collectibleId;

        /*private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(collectorName))
            {
                return;
            }
            Collect();
        }*/

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
            string sceneName = SceneController.Instance.activeSceneName;
            if (sceneName == "ForestTopScene")
            {
                var addItem = new ItemBagMessage
                {
                    clientId = "TopView",
                    type = "AddItem",
                    recipientId = "Android",
                    objectId = collectibleId,
                    sceneName = "forest"
                };
                WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addItem));
            }
            else
            {
                var addItem = new ItemBagMessage
                {
                    clientId = "TopView",
                    type = "AddItem",
                    recipientId = "Android",
                    objectId = collectibleId,
                    sceneName = "workshop"
                };
                WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addItem));
            }
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