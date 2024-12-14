using UnityEngine;

namespace Client
{
    public class SendTest : MonoBehaviour
    {
        [ContextMenu("Send")]
        public void Send()
        {
            WebSocketClient.Instance.SendMessageToServer("Hello, world!");
        }
        [ContextMenu("Vibrate")]
        public void Vibrate()
        {
            var vibrateObject = new Message
            {
                clientId = "TopView",
                type = "vibrate",
                recipientId = "Android"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(vibrateObject));
        }
    }
}