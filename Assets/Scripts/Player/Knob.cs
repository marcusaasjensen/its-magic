using UnityEngine;

namespace Player
{
    public class Knob : MonoBehaviour
    {
        [SerializeField] private int fingerId;
        [SerializeField] private GameObject knob;
        
        private Camera _camera;
        
        private void Start() => _camera = Camera.main;

        private void Update()
        {
            UpdateKnobVisibility();
            FollowFinger();
        }

        private void UpdateKnobVisibility()
        {
            knob.SetActive(TouchInput.Instance.IsTouching);
        }
    
        private void FollowFinger()
        {
            if (TouchInput.Instance.Touches.Length <= 0)
            {
                return;
            }
        
            var ray = _camera.ScreenPointToRay(TouchInput.Instance.Touches[fingerId].position);
        
            Debug.DrawRay(new Vector3(ray.origin.x, ray.origin.y, transform.position.z), ray.direction, Color.red);

            transform.position = new Vector2(ray.origin.x, ray.origin.y);
        }
    }
}
