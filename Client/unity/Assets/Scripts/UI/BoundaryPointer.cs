using UnityEngine;

namespace UI
{
    public class BoundaryPointer : MonoBehaviour
    {
        [SerializeField] private Transform boundary; // The boundary the GameObject interacts with

        private void Update()
        {
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            if (!boundary) return;

            // Get the spotlight's position
            Vector2 spotlightPosition = transform.position;

            // Get the center of the boundary
            Vector2 boundaryCenter = boundary.position;

            // Calculate the direction from the spotlight to the center of the boundary
            Vector2 directionToCenter = spotlightPosition - boundaryCenter;

            // Calculate the angle between the spotlight and the center of the boundary
            float angle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;

            // Apply the calculated rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}