using Client;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class DirectionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool isRight;
        [SerializeField] private UnityEvent onClickDown;
        [SerializeField] private UnityEvent onClickUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            onClickDown?.Invoke();
            UpdateArrow();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onClickUp?.Invoke();
            UpdateArrow();
        }

        private void UpdateArrow()
        {
            var message = new ArrowMessage { leftOrRight = isRight };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(message));
        }
    }
}