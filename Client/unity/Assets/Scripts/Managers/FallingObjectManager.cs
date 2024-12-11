using Client;
using UnityEngine;

namespace Managers
{
    public class FallingObjectManager : MonoBehaviour
    {
        [SerializeField] private GameObject fallenObjectPrefab;

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

            // Calculate the position based on the formula
            Vector2 topViewPosition = CalculatePosition(fallingObjectMessage.x);

            // Instantiate the object at the calculated position
            Instantiate(fallenObjectPrefab, new Vector3(topViewPosition.x, 0, topViewPosition.y), Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }

        private Vector2 CalculatePosition(float sideViewX)
        {
            // 1. Get the boundary center
            Vector2 boundaryCenter = boundaryTransform.position;

            // 2. Normalize the X position from the side view range (-40 to 40) to [0, 1]
            float normalizedX = Mathf.Clamp01((sideViewX - minX) / (maxX - minX));

            // 3. Calculate the total perimeter of the bounds
            float width = boundarySize.x;
            float height = boundarySize.y;
            float perimeter = 2 * (width + height);

            // 4. Map normalized X to a position along the perimeter
            float perimeterOffset = normalizedX * perimeter;

            // 5. Use formulas to calculate the exact position
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
