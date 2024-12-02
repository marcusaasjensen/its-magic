using Environment;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class Slasher : MonoBehaviour
    {
        [SerializeField] private float swipeSpeedThreshold = 5f;
        [SerializeField] private float slashRadius = 1f;
        [SerializeField] private UnityEvent onAnySlash;

        private Vector3 _swipeStartPosition;
        private float _swipeStartTime;

        private void Update()
        {
            DetectSlash();
        }

        private void DetectSlash()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    _swipeStartPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                    _swipeStartPosition.z = 0;
                    _swipeStartTime = Time.time;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Vector3 swipeEndPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                    swipeEndPosition.z = 0;
                    float swipeEndTime = Time.time;

                    float swipeDistance = Vector3.Distance(_swipeStartPosition, swipeEndPosition);
                    float swipeDuration = swipeEndTime - _swipeStartTime;
                    float swipeSpeed = swipeDistance / swipeDuration;

                    if (swipeSpeed >= swipeSpeedThreshold)
                    {
                        DetectSlashableObjects(_swipeStartPosition, swipeEndPosition);
                    }
                }
            }
        }

        private void DetectSlashableObjects(Vector3 swipeStart, Vector3 swipeEnd)
        {
            Vector3 swipeDirection = (swipeEnd - swipeStart).normalized;

            RaycastHit2D[] hitColliders = Physics2D.RaycastAll(swipeStart, swipeDirection, slashRadius);

            foreach (var hit in hitColliders)
            {
                ISlashable slashable = hit.collider.GetComponent<ISlashable>();
                if (slashable != null)
                {
                    slashable.Slash();
                    onAnySlash.Invoke();
                }
            }
        }
    }
}
