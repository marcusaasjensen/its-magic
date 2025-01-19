using Client;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private UnityEvent onExplode; 
        public void Explode(string message)
        {
            var potionMessage = JsonUtility.FromJson<PotionMessage>(message);
            
            if( potionMessage is { type: "Potion" })
            {
                Debug.Log("Potion Exploded");
                onExplode.Invoke();
            }
        }
        
        public void ExplodeNow()
        {
            Debug.Log("Potion Exploded");
            onExplode.Invoke();
        }
        
    }
}