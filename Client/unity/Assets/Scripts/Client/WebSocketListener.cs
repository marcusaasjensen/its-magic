using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Client
{
    public class WebSocketListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onMessageReceived;
        
        private void OnEnable()
        {
            WebSocketClient.Instance.onMessageReceived.AddListener(ReceiveMessage);
        }
        
        private void OnDisable()
        {
            WebSocketClient.Instance.onMessageReceived.RemoveListener(ReceiveMessage);
        }

        private void ReceiveMessage(string message)
        {
            onMessageReceived.Invoke(message);
        }
    }
}