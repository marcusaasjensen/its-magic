using System;
using Managers;
using UnityEngine;

namespace Environment
{
    public class Draggable : MonoBehaviour
    {
        private Vector2 _touchOffset; // Offset between touch and object position
        private int _activeTouchId = -1; // ID of the touch dragging this object
        protected bool IsBeingDragged => _activeTouchId != -1;
        public bool IsDraggable { get; set; } = true;

        protected void Update()
        {
            if (!IsDraggable)
            {
                _activeTouchId = -1;
                print(IsDraggable);
                return;
            }
            // Iterate through all active touches
            foreach (Touch touch in Input.touches)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    // Check if this touch began on this object
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                    if (hit.collider != null && hit.collider.name == gameObject.name)
                    {
                        _activeTouchId = touch.fingerId; // Assign the touch ID
                        _touchOffset = (Vector2)transform.position - touchPosition;
                    }
                }
                else if (touch.fingerId == _activeTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        // Drag the object with the touch
                        transform.position = (Vector2)touchPosition + _touchOffset;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        // Release the object when touch ends
                        _activeTouchId = -1;
                    }
                }
            }
        }
    }
}