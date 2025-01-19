using Client;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] private Cauldron cauldron;
        [SerializeField] private UnityEvent onPotionFinished;
        [SerializeField] private UnityEvent onPotionExplode;
        [SerializeField] private UnityEvent onPotionReset; 
        
        public bool IsPotionReady { get; private set; }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PotionCatcher") && !IsPotionReady)
            {
                onPotionReset.Invoke();
                return;
            }

            if (other.CompareTag("PotionCatcher") && IsPotionReady)
            {
                Explode();
            }
            if (!other.CompareTag("Cauldron") || !cauldron.IsPotionReady || IsPotionReady) return;
            onPotionFinished.Invoke();
            IsPotionReady = true;
        }
        
        public void Explode()
        {
            var potionMessage = new PotionMessage();
            onPotionExplode.Invoke();
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(potionMessage));
        }
    }
}