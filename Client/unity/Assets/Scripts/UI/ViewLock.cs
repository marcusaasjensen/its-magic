using Client;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ViewLock : MonoBehaviour
    {
        [SerializeField] private UnityEvent onLock;
        [SerializeField] private UnityEvent onUnlock;
        private bool _isLocked;
        
        public void ToggleLock()
        {
            _isLocked = !_isLocked;
            
            if (_isLocked)
            {
                onLock.Invoke();
            }
            else
            {
                onUnlock.Invoke();
            }
            
            var lockMessage = new LockMessage
            {
                isLocked = _isLocked
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(lockMessage));
        }
        
    }
}