using Client;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class FallingObjectCatcher : MonoBehaviour
    {
        [SerializeField] public UnityEvent<FallingObject> onFallingObject;
        private void OnTriggerEnter2D(Collider2D other)
        {
            var fallingObject = other.GetComponent<FallingObject>();
            
            if (fallingObject == null)
            {
                return;
            }

            onFallingObject.Invoke(fallingObject);
            
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