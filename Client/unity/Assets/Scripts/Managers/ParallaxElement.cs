using UnityEngine;

namespace Managers
{
    public class ParallaxElement : MonoBehaviour
    {
        [SerializeField] private float scrollSpeedMultiplier = 1f; // Speed adjustment via mouse scroll
        private float screenWidth; // Width of a single screen in world units
        private int totalScreens = 4; // Total number of screens in the 360-degree loop
        private Vector3 initialPosition; // Original position of the object

        private Camera mainCamera;
        private float scrollAngle = 0f; // Global scroll angle controlled by ParallaxManager

        void Start()
        {
            // Store the object's initial position
            initialPosition = transform.position;

            // Get the main camera
            mainCamera = Camera.main;

            // Calculate the screen's width based on its renderer bounds
            Renderer renderer = GetComponent<Renderer>();
            if (renderer)
            {
                screenWidth = renderer.bounds.size.x;
            }
            else
            {
                Debug.LogError("No Renderer found on the object. Ensure the object has a Renderer component.");
            }
        }

        void Update()
        {
            // Apply the parallax effect based on the global scroll angle
            ApplyParallaxEffect();
            HandleTeleportation();
        }

        // Set the global scroll angle (called by ParallaxManager)
        public void SetScrollAngle(float angle)
        {
            scrollAngle = angle;
        }

        private void ApplyParallaxEffect()
        {
            float normalizedScroll = scrollAngle / 360f;
            float totalWidth = screenWidth * totalScreens;
            float offset = totalWidth * normalizedScroll;

            offset = Mathf.Repeat(offset, totalWidth);
            transform.position = new Vector3(initialPosition.x + offset, initialPosition.y, initialPosition.z);
        }

        private void HandleTeleportation()
        {
            float cameraLeftBound = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
            float cameraRightBound = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;

            if (transform.position.x + screenWidth / 2 < cameraLeftBound)
            {
                Teleport(screenWidth * totalScreens);
            }
            else if (transform.position.x - screenWidth / 2 > cameraRightBound)
            {
                Teleport(-screenWidth * totalScreens);
            }
        }

        private void Teleport(float offset)
        {
            transform.position += new Vector3(offset, 0, 0);
        }
    }
}
