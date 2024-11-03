using System.Collections.Generic;
using System.Linq;
using Environment;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class SelectableManager : Utils.SceneSingleton<SelectableManager>
    {
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        public List<Selectable> Selectables { get; private set; } = new List<Selectable>();

        private Vector3 _selectionCenter;
        private Vector3 _lastTouchPosition;
        private float _lastTouchAngle = 0f;
        private bool _rotationInProgress = false;

        private void Update()
        {
            if (Input.touchCount <= 0) return;

            // Calculate average touch position for movement
            Vector2 averageTouchPosition = Vector2.zero;
            for (var i = 0; i < Input.touchCount; i++)
            {
                averageTouchPosition += Input.GetTouch(i).position;
            }
            averageTouchPosition /= Input.touchCount;

            // Convert screen position to world point
            var currentTouchPosition = Camera.main.ScreenToWorldPoint(new Vector3(averageTouchPosition.x, averageTouchPosition.y, 0f));
            currentTouchPosition.z = 0;

            // Initialize last touch position if first touch frame
            if (_lastTouchPosition == Vector3.zero)
            {
                _lastTouchPosition = currentTouchPosition;
                CalculateSelectionCenter();
            }

            // Calculate movement delta and move selected objects
            Vector3 movementDelta = currentTouchPosition - _lastTouchPosition;
            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                selectable.transform.position += movementDelta;
            }

            // Handle rotation if there are at least two fingers
            if (Input.touchCount >= 2)
            {
                if (!_rotationInProgress)
                {
                    // Lock the selection center and start rotation tracking
                    CalculateSelectionCenter();
                    _rotationInProgress = true;
                }

                HandleTwoFingerRotation();
            }
            else
            {
                // Reset rotation state when fewer than two fingers
                _lastTouchAngle = 0f;
                _rotationInProgress = false;
            }

            // Update last touch position
            _lastTouchPosition = currentTouchPosition;
        }

        private void HandleTwoFingerRotation()
        {
            // Get positions of the first two touches
            var touch0 = Input.GetTouch(0).position;
            var touch1 = Input.GetTouch(1).position;

            // Calculate the current angle between the two touches
            float currentAngle = Mathf.Atan2(touch1.y - touch0.y, touch1.x - touch0.x) * Mathf.Rad2Deg;

            // If this is the first frame with two fingers, initialize last angle
            if (_lastTouchAngle == 0f)
            {
                _lastTouchAngle = currentAngle;
                return;
            }

            // Calculate the angle difference (rotation)
            float angleDelta = currentAngle - _lastTouchAngle;

            // Apply rotation to each selected object around the locked selection center
            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                selectable.transform.RotateAround(_selectionCenter, Vector3.forward, angleDelta);
            }

            // Update the selection center based on the new positions of selected objects
            CalculateSelectionCenter();

            // Update last angle for the next frame
            _lastTouchAngle = currentAngle;
        }

        public void RegisterSelectable(Selectable selectable, bool register = true)
        {
            if (register)
            {
                Selectables.Add(selectable);
                onSelected.Invoke();
                CalculateSelectionCenter();
            }
            else
            {
                Selectables.Remove(selectable);
                onDeselected.Invoke();
                CalculateSelectionCenter();
            }
        }

        public void UnregisterSelectable(Selectable selectable)
        {
            if (Selectables.Contains(selectable))
            {
                Selectables.Remove(selectable);
                onDeselected.Invoke();
                CalculateSelectionCenter();
            }
        }

        private void CalculateSelectionCenter()
        {
            if (Selectables.Count == 0) return;

            // Calculate the center point of selected objects
            _selectionCenter = Vector3.zero;
            int selectedCount = 0;

            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                _selectionCenter += selectable.transform.position;
                selectedCount++;
            }

            _selectionCenter /= selectedCount > 0 ? selectedCount : 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_selectionCenter, 1f);
        }
    }
}
