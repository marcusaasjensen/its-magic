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
        
        [ContextMenu("Light")]
        public void getClock()
        {
            var getClock = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "alarmClock",
                objectScene = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getClock));
        }
        
        [ContextMenu("Bag")]
        public void getBag()
        {
            var getBag = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "bag",
                objectScene = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getBag));
        }
    }
}