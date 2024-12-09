using Client;
using Environment;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player
{
    public class MagicWand : Draggable
    {
        [SerializeField] private Sprite magicWand;
        [SerializeField] private Sprite lockedMagicWand;
        [SerializeField] public UnityEvent onUnlock;
        [SerializeField] public UnityEvent onLock;

        private bool _isLocked;
        private float _currentRotationInDegrees;
        private SpriteRenderer _magicWandSpriteRenderer;
        
        private void Start()
        {
            var childTransform = transform.Find("magic_wand");
            if (childTransform != null)
            {
                _magicWandSpriteRenderer = childTransform.GetComponent<SpriteRenderer>();
            }
        }

        public void Lock(string message)
        {
            if(message == null)
            {
                return;
            }
            
            var lockMessage = JsonUtility.FromJson<LockMessage>(message);
            if(lockMessage is not { type: "Lock" })
            {
                return;
            }
            
            _isLocked = lockMessage.isLocked;
            IsDraggable = !_isLocked;

            if (_isLocked)
            {
                onLock.Invoke();
            }
            else
            {
                onUnlock.Invoke();
            }
        }
        
        private new void Update()
        {
            base.Update();
            
            _magicWandSpriteRenderer.sprite = _isLocked ? lockedMagicWand : magicWand;

            if (_isLocked)
            {
                return;
            }
            _currentRotationInDegrees = transform.rotation.eulerAngles.z < 0 ? 360 + transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;

            var magicStickMessage = new MagicStickMessage
            {
                rotationInDegrees = _currentRotationInDegrees
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(magicStickMessage));
        }
    }
}