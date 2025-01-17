using UnityEngine.Events;

namespace Managers
{
    using UnityEngine;

    public class SnapToPoint : MonoBehaviour
    {
        [SerializeField] private Transform toSnap;
        [SerializeField] private Transform snapPoint;
        [SerializeField] private Transform unsnapPoint;
        [SerializeField] private float snapDistance = 1f;
        [SerializeField] private GameObject snapShadow;
        [SerializeField] private UnityEvent onSnap;
        [SerializeField] private UnityEvent onUnsnap;

        private bool _isSnapped;

        private void Update()
        {
            if (snapShadow == null) return;

            snapShadow.SetActive(!_isSnapped && Vector3.Distance(toSnap.position, snapPoint.position) <= snapDistance);
        }

        public void SnapToPosition()
        {
            if (toSnap == null || snapPoint == null) return;

            if (Vector3.Distance(toSnap.position, snapPoint.position) <= snapDistance && !_isSnapped)
            {
                toSnap.position = snapPoint.position;
                _isSnapped = true;
                onSnap.Invoke();
                Debug.Log("Snapped to the snap point.");
            }

            if (Vector3.Distance(toSnap.position, snapPoint.position) > snapDistance && _isSnapped)
            {
                _isSnapped = false;
                onUnsnap.Invoke();
            }
        }

        public void Unsnap()
        {
            if (toSnap == null || unsnapPoint == null || !_isSnapped) return;
            toSnap.position = unsnapPoint.position;
            _isSnapped = false;
            onUnsnap.Invoke();
        }
    }
}