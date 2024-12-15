using Client;
using UnityEngine;

namespace Player
{
    public class Vibrator : MonoBehaviour
    {
        public void Vibrate()
        {
            var message = new Message
            {
                clientId = "TopView",
                type = "vibrate",
                recipientId = "Android"
            };
            
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(message));
        }
    }
}