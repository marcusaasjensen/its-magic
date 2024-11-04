using System;
using Managers;
using UnityEngine;

namespace Environment
{
    public class Draggable : MonoBehaviour
    {
        [SerializeField] private string draggableTag = "Draggable";
        private bool _isDragging;
        private bool _canDrag;
        private Vector3 _offset;

        private void Update()
        {
            HandleMouseDrag();
        }

        public void HandleMouseDrag()
        {
            Vector2 mousePosition = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse is clicking and dragging a draggable object
            if (Input.GetMouseButtonDown(0))  // Only calculate offset when mouse is pressed down
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag(draggableTag))
                {
                    _isDragging = true;
                    _canDrag = hit.collider.gameObject == gameObject;

                    // Calculate offset between object position and mouse click position
                    if (_canDrag)
                    {
                        Vector3 hitPoint = hit.point;
                        _offset = hit.collider.transform.position - hitPoint;
                    }
                }
            }

            // If currently dragging, apply the offset
            if (_isDragging && _canDrag && Input.GetMouseButton(0))
            {
                transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z) + _offset;
            }

            // Stop dragging when mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
        }

        private void OnDisable()
        {
            _isDragging = false;
        }
    }
}