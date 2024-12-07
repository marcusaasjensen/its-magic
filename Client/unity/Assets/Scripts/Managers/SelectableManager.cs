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
        private float _lastTouchAngle;
        private bool _rotationInProgress;
        private float _lastTouchDistance;

        [SerializeField] private Vector2 minBounds = new (-5, -5);
        [SerializeField] private Vector2 maxBounds = new (5, 5);

        private void Update()
        {
            if (Input.touchCount <= 0) return;

            Vector2 averageTouchPosition = Vector2.zero;
            for (var i = 0; i < Input.touchCount; i++)
            {
                averageTouchPosition += Input.GetTouch(i).position;
            }
            averageTouchPosition /= Input.touchCount;

            var currentTouchPosition = Camera.main.ScreenToWorldPoint(new Vector3(averageTouchPosition.x, averageTouchPosition.y, 0f));
            currentTouchPosition.z = 0;

            if (_lastTouchPosition == Vector3.zero)
            {
                _lastTouchPosition = Vector3.Lerp(_lastTouchPosition, currentTouchPosition, Time.deltaTime * 10);
                CalculateSelectionCenter();
            }

            Vector3 movementDelta = currentTouchPosition - _lastTouchPosition;
            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                selectable.transform.position += movementDelta;
                selectable.transform.position = ClampToBounds(selectable.transform.position);
            }

            if (Input.touchCount >= 2)
            {
                if (!_rotationInProgress)
                {
                    CalculateSelectionCenter();
                    _rotationInProgress = true;
                }

                HandleTwoFingerRotation();
                HandlePinchGesture();
            }
            else
            {
                _lastTouchAngle = 0f;
                _lastTouchDistance = 0f; // Reset the last distance when fingers are lifted
                _rotationInProgress = false;
            }
            _lastTouchPosition = currentTouchPosition;
        }

        private void HandleTwoFingerRotation()
        {
            var touch0 = Input.GetTouch(0).position;
            var touch1 = Input.GetTouch(1).position;

            float currentAngle = Mathf.Atan2(touch1.y - touch0.y, touch1.x - touch0.x) * Mathf.Rad2Deg;

            if (_lastTouchAngle == 0f)
            {
                _lastTouchAngle = currentAngle;
                return;
            }
            
            float angleDelta = currentAngle - _lastTouchAngle;

            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                selectable.transform.RotateAround(_selectionCenter, Vector3.forward, angleDelta);
                selectable.transform.position = ClampToBounds(selectable.transform.position);
            }

            CalculateSelectionCenter();

            _lastTouchAngle = currentAngle;
        }

        private void HandlePinchGesture()
        {
            var touch0 = Input.GetTouch(0).position;
            var touch1 = Input.GetTouch(1).position;

            float currentDistance = Vector2.Distance(touch0, touch1);

            if (_lastTouchDistance == 0f)
            {
                _lastTouchDistance = currentDistance;
                return;
            }

            // Calculate the difference in distance and adjust sensitivity
            float distanceDelta = currentDistance - _lastTouchDistance;
            float scaleFactor = 1 + (distanceDelta * 0.001f); // Lower sensitivity to 0.001f

            // Clamp scale factor to prevent extreme scaling
            scaleFactor = Mathf.Clamp(scaleFactor, 0.95f, 1.05f); // Limits how much items can scale per frame

            foreach (var selectable in Selectables.Where(s => s.IsSelected))
            {
                // Scale each selected item relative to the selection center
                Vector3 direction = selectable.transform.position - _selectionCenter;
                selectable.transform.position = ClampToBounds(_selectionCenter + direction * scaleFactor);
            }

            CalculateSelectionCenter();

            _lastTouchDistance = currentDistance;
        }

        private Vector3 ClampToBounds(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
            return position;
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

        private void CalculateSelectionCenter()
        {
            if (Selectables.Count == 0) return;

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

            // Draw bounds for visual debugging
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, minBounds.y, 0));
            Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, maxBounds.y, 0));
            Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, maxBounds.y, 0));
            Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, minBounds.y, 0));
        }
    }
}
