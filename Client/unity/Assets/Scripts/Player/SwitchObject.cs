using Client;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class SwitchObject : MonoBehaviour
    {
        [SerializeField] 
        private string objectNameToShow;
        [SerializeField] 
        private string sceneName;

        public void ShowObject()
        {
            var objectMessage = new SwitchObjectMessage
            {
                objectName = objectNameToShow,
                objectScene = sceneName
            };

            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(objectMessage));
        }
    }
}