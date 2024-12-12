using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace UI
{
    public class DepthOfFieldControl : MonoBehaviour
    {
        public Volume volume; // Reference to the post-processing volume
        public float focusDistanceNear = 5f;
        public float focusDistanceFar = 20f;
        private DepthOfField depthOfField;

        public LayerMask layerMask; // Layer mask to define which layers are affected by Depth of Field

        void Start()
        {
            // Check if volume exists and fetch Depth of Field effect
            if (volume.profile.TryGet(out depthOfField))
            {
                Debug.Log("Depth of Field found!");
            }
            else
            {
                Debug.LogError("Depth of Field effect not found in the volume profile.");
            }
        }

        void Update()
        {
            // Apply Depth of Field based on the layers in the scene
            ApplyDepthOfFieldBasedOnLayer();
        }

        void ApplyDepthOfFieldBasedOnLayer()
        {
            // Get all objects in the specified layers
            GameObject[] objectsInLayer = GetObjectsInLayer(layerMask);

            // Loop through each object in the layer and adjust focus distance based on its position
            foreach (GameObject obj in objectsInLayer)
            {
                if (obj != null)
                {
                    // Calculate the object's Z position (depth) to adjust focus distance
                    float zPosition = obj.transform.position.z;

                    // Adjust the focus distance based on object's Z position and layer-specific range
                    if (zPosition < focusDistanceNear)
                        depthOfField.focalLength.value = focusDistanceNear;
                    else if (zPosition > focusDistanceFar)
                        depthOfField.focalLength.value = focusDistanceFar;
                    else
                        depthOfField.focalLength.value = zPosition;
                }
            }
        }

        // Helper function to get all objects in the specified layer mask
        GameObject[] GetObjectsInLayer(LayerMask layerMask)
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            var objectsInLayer = new System.Collections.Generic.List<GameObject>();

            foreach (var obj in allObjects)
            {
                if ((layerMask & (1 << obj.layer)) != 0)
                {
                    objectsInLayer.Add(obj);
                }
            }

            return objectsInLayer.ToArray();
        }
    }
}
