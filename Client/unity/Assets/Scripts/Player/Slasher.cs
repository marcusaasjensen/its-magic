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
        [SerializeField] private GameObject slashFollower; // The referenced GameObject to follow the slash
        [SerializeField] private float trailVisibilityDuration = 0.2f; // Time in seconds to keep the trail visible after swipe

        private Vector3 _swipeStartPosition;
        private float _swipeStartTime;
        private bool _isSwiping;
        private bool _isSlashing;

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
                    _isSwiping = true;
                    _isSlashing = false;

                    if (slashFollower != null)
                    {
                        slashFollower.SetActive(false); // Ensure the trail is inactive initially
                    }
                }
                else if (touch.phase == TouchPhase.Moved && _isSwiping)
                {
                    Vector3 swipeCurrentPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0f));
                    swipeCurrentPosition.z = 0;

                    float swipeDistance = Vector3.Distance(_swipeStartPosition, swipeCurrentPosition);
                    float swipeDuration = Time.time - _swipeStartTime;
                    float swipeSpeed = swipeDistance / swipeDuration;

                    if (swipeSpeed >= swipeSpeedThreshold)
                    {
                        _isSlashing = true;

                        if (slashFollower != null && !slashFollower.activeSelf)
                        {
                            slashFollower.transform.position = swipeCurrentPosition;
                            slashFollower.SetActive(true);
                        }
                    }

                    // Update the slash follower's position only while slashing
                    if (slashFollower != null && _isSlashing)
                    {
                        slashFollower.transform.position = swipeCurrentPosition;
                    }
                }
                else if (touch.phase == TouchPhase.Ended && _isSwiping)
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

                    // Reset swipe tracking
                    _isSwiping = false;
                    _isSlashing = false;

                    // Deactivate the slash follower only if it was active
                    if (slashFollower != null && slashFollower.activeSelf)
                    {
                        Invoke(nameof(DeactivateSlashFollower), trailVisibilityDuration);
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

        private void DeactivateSlashFollower()
        {
            if (slashFollower != null)
            {
                slashFollower.SetActive(false);
            }
        }
    }
}
