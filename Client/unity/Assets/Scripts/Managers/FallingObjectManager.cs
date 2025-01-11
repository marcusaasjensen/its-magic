using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Managers
{
    public class FallingObjectManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> fallenObjectPrefab;

        [SerializeField] private Transform boundaryTransform; // Center of the squarish shape
        [SerializeField] private Vector2 boundarySize = new(10, 10); // Size of the squarish bounds

        [SerializeField] private float minX = -40f; // Minimum x value in the side view
        [SerializeField] private float maxX = 40f;  // Maximum x value in the side view

        public void Fall(string message)
        {
            if (message == null)
            {
                return;
            }

            var fallingObjectMessage = JsonUtility.FromJson<FallingObjectMessage>(message);
            if (fallingObjectMessage is not { type: "FallingObject" })
            {
                return;
            }
            
            var topViewPosition = CalculatePosition(fallingObjectMessage.x);
            var prefab = fallenObjectPrefab.Find( x => x.name == fallingObjectMessage.name);

            Instantiate(prefab, new Vector3(topViewPosition.x, 0, topViewPosition.y), Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
        }

        private Vector2 CalculatePosition(float sideViewX)
        {
            Vector2 boundaryCenter = boundaryTransform.position;
            
            float normalizedX = Mathf.Clamp01((sideViewX - minX) / (maxX - minX));
            
            float width = boundarySize.x;
            float height = boundarySize.y;
            float perimeter = 2 * (width + height);
            
            float perimeterOffset = normalizedX * perimeter;
            
            if (perimeterOffset <= width) // Top edge
            {
                return new Vector2(boundaryCenter.x - width / 2 + perimeterOffset, boundaryCenter.y + height / 2);
            }
            else if (perimeterOffset <= width + height) // Right edge
            {
                return new Vector2(boundaryCenter.x + width / 2, boundaryCenter.y + height / 2 - (perimeterOffset - width));
            }
            else if (perimeterOffset <= 2 * width + height) // Bottom edge
            {
                return new Vector2(boundaryCenter.x + width / 2 - (perimeterOffset - (width + height)), boundaryCenter.y - height / 2);
            }
            else // Left edge
            {
                return new Vector2(boundaryCenter.x - width / 2, boundaryCenter.y - height / 2 + (perimeterOffset - (2 * width + height)));
            }
        }
    }
}
