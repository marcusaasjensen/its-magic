using Client;
using UnityEngine;

namespace UI
{
    public class ViewLock : MonoBehaviour
    {
        private bool _isLocked;
        
        public void ToggleLock()
        {
            _isLocked = !_isLocked;
            
            var lockMessage = new LockMessage
            {
                isLocked = _isLocked
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(lockMessage));
        }
        
    }
}