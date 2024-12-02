using Client;
using UnityEngine;

namespace Environment
{
    public class FallingObjectCatcher : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var fallingObject = other.GetComponent<FallingObject>();
            
            if (fallingObject == null)
            {
                return;
            }
            
            var fallingObjectMessage = new FallingObjectMessage
            {
                id = fallingObject.GetInstanceID(),
                name = fallingObject.Name,
                x = fallingObject.transform.position.x,
            };
            
            Destroy(fallingObject.gameObject);
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(fallingObjectMessage));
        }
    }
}