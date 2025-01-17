using Client;
using UnityEngine;

namespace Environment
{
    public class Cauldron : MonoBehaviour
    {
        [SerializeField] private GameObject sideCauldron;
        public void PositionCauldron()
        {
            var cauldronMessage = new CauldronSnapMessage
            {
                isSnapped = true
            };
            
            WebSocketClient.Instance.SendMessage(JsonUtility.ToJson(cauldronMessage));
        }
        
        public void RemoveCauldron()
        {
            var cauldronMessage = new CauldronSnapMessage
            {
                isSnapped = false
            };
            
            WebSocketClient.Instance.SendMessage(JsonUtility.ToJson(cauldronMessage));
        }

        public void UpdateSideCauldron(string message)
        {
            var cauldronSnapMessage = JsonUtility.FromJson<CauldronSnapMessage>(message);
            if(cauldronSnapMessage is not { type: "CauldronSnap" })
            {
                return;
            }
            
            sideCauldron.SetActive(cauldronSnapMessage.isSnapped);
        }
        
    }
}