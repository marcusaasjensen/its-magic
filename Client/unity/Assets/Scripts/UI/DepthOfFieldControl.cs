using Client;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace UI
{
    public class DepthOfFieldControl : MonoBehaviour
    {
        [Header("Post-Processing Settings")]
        [SerializeField] private Volume postProcessingVolume;
        [SerializeField, Min(0.1f)] private float minFocusDistance = 1.9f;
        [SerializeField, Min(0.1f)] private float maxFocusDistance = 9.8f;
        public float distanceToCenter;
        private DepthOfField _depthOfField;

        [Header("Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField, Min(0)] private float minOrthoSize;
        [SerializeField, Min(0)] private float maxOrthoSize = 5f;

        private void Start()
        {
            if (postProcessingVolume == null || !postProcessingVolume.profile.TryGet(out _depthOfField))
            {
                Debug.LogError("Depth of Field component not found in the post-processing profile.");
            }
            
            _depthOfField.active = true;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            else if (!mainCamera.orthographic)
            {
                Debug.LogError("Assigned camera is not orthographic. Please use an orthographic camera.");
            }
        }

        public void ChangeFocus(string message)
        {
            if (message == null || _depthOfField == null || mainCamera == null)
            {
                return;
            }

            var magicWandMessage = JsonUtility.FromJson<MagicWandMessage>(message);

            if (magicWandMessage is not { type: "MagicWand" })
            {
                return;
            }

            distanceToCenter = magicWandMessage.distanceToCenter;
        }

        private void Update()
        {
            ApplyDepthOfFieldAndZoom(distanceToCenter);
        }

        private void ApplyDepthOfFieldAndZoom(float distance)
        {
            var t = Mathf.InverseLerp(minFocusDistance, maxFocusDistance, distance);
            var focusDistance = Mathf.Lerp(maxFocusDistance, minFocusDistance, t);
            _depthOfField.focusDistance.value = focusDistance;

            _depthOfField.aperture.value = Mathf.Lerp(16f, 1.4f, t); // Wide aperture for blur

            var targetOrthoSize = Mathf.Lerp(maxOrthoSize, minOrthoSize, t);
            mainCamera.orthographicSize = targetOrthoSize;
        }
    }
}
