using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class Knob : MonoBehaviour
    {
        [field: SerializeField] public int FingerId { get; set; }
        [SerializeField] private GameObject knob;
        
        private Camera _camera;
        
        public bool IsVisible => knob.activeSelf;
        
        private void Awake()
        {
            UpdateCamera();
            CameraManager.Instance.OnCameraSwitch += UpdateCamera;
        }

        private void OnDestroy()
        {
            CameraManager.Instance.OnCameraSwitch -= UpdateCamera;
        }

        private void UpdateCamera()
        {
            _camera = Camera.main;
        }

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
            
            var ray = _camera.ScreenPointToRay(TouchInput.Instance.Touches[FingerId].position);
        
            Debug.DrawRay(new Vector3(ray.origin.x, ray.origin.y, transform.position.z), ray.direction, Color.red);

            transform.position = new Vector2(ray.origin.x, ray.origin.y);
        }
    }
}
