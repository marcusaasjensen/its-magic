using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Clickable : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClick;

        private void Update()
        {
            if (Input.touchCount <= 0) return;

            // Get the first touch
            var touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began) return;

            // Convert the touch position to world space
            var worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
            var hit = Physics2D.OverlapPoint(worldPosition);

            // Check if the hit object is this object and if it has a Collider2D with isTrigger enabled
            if (hit != null && hit.isTrigger && hit.transform == transform)
            {
                // Trigger the onClick event
                onClick?.Invoke();
            }
        }
    }
}