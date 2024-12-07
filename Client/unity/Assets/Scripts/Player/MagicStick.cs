using Client;
using Environment;
using UnityEngine;

namespace Player
{
    public class MagicStick : Draggable
    {
        private float _currentRotationInDegrees;
        
        private new void Update()
        {
            base.Update();
            if (!IsBeingDragged)
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