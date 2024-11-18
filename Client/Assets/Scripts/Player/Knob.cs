using System;
using Environment;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class Knob : MonoBehaviour
    {
        [field: SerializeField] public int FingerId { get; set; }
        [SerializeField] private GameObject knob;
        
        public bool IsVisible => knob.activeSelf;

        private void Update()
        {
            UpdateKnobVisibility();
            FollowFinger();
        }

        private void UpdateKnobVisibility()
        {
            knob.SetActive(TouchInput.Instance.Touches.Length > FingerId);
        }
    
        private void FollowFinger()
        {
            if (TouchInput.Instance.Touches.Length <= FingerId)
            {
                return;
            }
            
            var ray = CameraManager.Instance.MainCamera.ScreenPointToRay(TouchInput.Instance.Touches[FingerId].position);
        
            Debug.DrawRay(new Vector3(ray.origin.x, ray.origin.y, transform.position.z), ray.direction, Color.red);

            transform.position = new Vector2(ray.origin.x, ray.origin.y);
        }
    }
}
