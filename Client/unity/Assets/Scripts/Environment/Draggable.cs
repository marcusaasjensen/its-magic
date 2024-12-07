using System;
using Managers;
using UnityEngine;

namespace Environment
{
    public class Draggable : MonoBehaviour
    {
        private Vector2 touchOffset; // Offset between touch and object position
        private int activeTouchId = -1; // ID of the touch dragging this object

        void Update()
        {
            // Iterate through all active touches
            foreach (Touch touch in Input.touches)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    // Check if this touch began on this object
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        activeTouchId = touch.fingerId; // Assign the touch ID
                        touchOffset = (Vector2)transform.position - touchPosition;
                    }
                }
                else if (touch.fingerId == activeTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        // Drag the object with the touch
                        transform.position = (Vector2)touchPosition + touchOffset;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        // Release the object when touch ends
                        activeTouchId = -1;
                    }
                }
            }
        }
    }
}