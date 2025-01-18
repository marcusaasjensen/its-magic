using System;
using Client;
using UnityEngine;

namespace Managers
{
    public class FireTrigger : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bellows"))
            {
                EnableBlow(true);
            }
        }
        
        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Bellows"))
            {
                EnableBlow(false);
            }
        }
        
        private static void EnableBlow(bool blow)
        {
            var bellowMessage = new BellowsMessage
            {
                isBlowingInFire = blow
            };
            
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(bellowMessage));
        }
    }
}