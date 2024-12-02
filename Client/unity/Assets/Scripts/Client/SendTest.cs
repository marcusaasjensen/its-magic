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
    }
}