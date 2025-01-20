using System;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Draggable : MonoBehaviour
    {
        [SerializeField] private UnityEvent onStartDrag;
        [SerializeField] private UnityEvent onDragged;
        [SerializeField] private UnityEvent onEndDrag; // Event for when dragging ends
        //[SerializeField] private LayerMask layer = 0;

        private Vector2 _touchOffset;
        private int _activeTouchId = -1;

        protected bool IsBeingDragged => _activeTouchId != -1;
        public bool IsDraggable { get; set; } = true;

        protected virtual void Update()
        {
            if (!IsDraggable)
            {
                _activeTouchId = -1;
                return;
            }

            foreach (Touch touch in Input.touches)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                if (touch.phase == TouchPhase.Began)
                {
                    var hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                    
                    if (hit.collider == null || hit.collider.name != gameObject.name)
                    {
                        print(hit.collider);
                        continue;
                    }

                    _activeTouchId = touch.fingerId; // Assign the touch ID
                    _touchOffset = (Vector2)transform.position - touchPosition;
                    onStartDrag?.Invoke();
                }
                else if (touch.fingerId == _activeTouchId)
                {
                    if (touch.phase is TouchPhase.Moved or TouchPhase.Stationary)
                    {
                        var targetPosition = (Vector2)touchPosition + _touchOffset;
                        SetPosition(targetPosition);
                    }
                    else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
                    {
                        _activeTouchId = -1;
                        onEndDrag?.Invoke(); // Invoke the onEndDrag event
                    }
                }
            }
        }

        protected virtual void SetPosition(Vector2 targetPosition)
        {
            transform.position = targetPosition;
            onDragged?.Invoke();
        }
    }
}
