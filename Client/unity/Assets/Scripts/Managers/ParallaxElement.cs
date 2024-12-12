using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class ParallaxElement : MonoBehaviour
    {
        [SerializeField] private float parallaxFactor = 1;
        private float parallaxEffect; // Width of a single screen in world units
        private int totalScreens = 4; // Total number of screens in the 360-degree loop
        private Vector3 initialPosition; // Original position of the object

        private Camera mainCamera;
        private float scrollAngle = 0f;
        private const float ScreenWidth = 17.8752f;

        private void Start()
        {
            initialPosition = transform.position;

            mainCamera = Camera.main;
            
            parallaxEffect = ScreenWidth * parallaxFactor;
        }

        private void Update()
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
            float totalWidth = parallaxEffect * totalScreens;
            float offset = totalWidth * normalizedScroll;

            offset = Mathf.Repeat(offset, totalWidth);
            transform.position = new Vector3(initialPosition.x + offset, initialPosition.y, initialPosition.z);
        }

        private void HandleTeleportation()
        {
            float cameraLeftBound = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
            float cameraRightBound = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;

            if (transform.position.x + parallaxEffect / 2 < cameraLeftBound)
            {
                Teleport(parallaxEffect * totalScreens);
            }
            else if (transform.position.x - parallaxEffect / 2 > cameraRightBound)
            {
                Teleport(-parallaxEffect * totalScreens);
            }
        }

        private void Teleport(float offset)
        {
            transform.position += new Vector3(offset, 0, 0);
        }
    }
}
