using Client;
using UnityEngine;

namespace Environment
{
    public class FallingObjectManager : MonoBehaviour
    {
        [SerializeField] private GameObject fallenObjectPrefab;
        
        private void Update()
        {
            var messageQueue = WebSocketClient.Instance.messageQueue;
            lock (messageQueue)
            {
                while (messageQueue.Count == 0)
                {
                    return;
                }
                var message = messageQueue.Dequeue();
                if(message == null)
                {
                    return;
                }
                var fallingObjectMessage = JsonUtility.FromJson<FallingObjectMessage>(message);
                if(fallingObjectMessage == null)
                {
                    return;
                }
                
                var fallenObject = Instantiate(fallenObjectPrefab, new Vector3(fallingObjectMessage.x, 0, 0), Quaternion.identity);
            }
        }
        
    }
}