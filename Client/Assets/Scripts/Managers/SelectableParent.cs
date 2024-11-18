using Player;
using UnityEngine;

namespace Managers
{
    public class SelectableParent : MonoBehaviour
    {
        private Vector3 _targetPosition;

        private void Update()
        {
            if (Input.touchCount <= 0)
            {
                return;
            }
            var averageTouchPosition = Vector2.zero;

            for (var i = 0; i < Input.touchCount; i++)
            {
                averageTouchPosition += Input.GetTouch(i).position;
            }
                
            averageTouchPosition /= Input.touchCount;

            _targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(averageTouchPosition.x, averageTouchPosition.y, 0f));

            _targetPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);
        }
    }
}