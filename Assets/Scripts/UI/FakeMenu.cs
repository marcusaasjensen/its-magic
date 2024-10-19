using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class FakeMenu : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private GameObject fakeMenu;
        [SerializeField] private Animator animator;
        [SerializeField] private List<FakeMenuItem> menuItems;

        private int _counter;
        private static readonly int IsOdd = Animator.StringToHash("isOdd");

        private bool _isDragging;
        private Vector2 _pointerOffset; // Offset between the pointer and menu position at start
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        public void EnabledAllMenuItems()
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.IsDraggable = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleMenu();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ToggleRotation();
            }
        }

        private void ToggleMenu()
        {
            fakeMenu.SetActive(!fakeMenu.activeSelf);
        }

        private void ToggleRotation()
        {
            animator.SetBool(IsOdd, _counter % 2 == 0);
            _counter++;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!fakeMenu.activeSelf || TouchInput.Instance.Selection.IsSelecting) return;

            _isDragging = true;

            // Capture the offset between the menu and the pointer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out Vector2 localPointerPos);

            _pointerOffset = _rectTransform.anchoredPosition - localPointerPos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || TouchInput.Instance.Selection.IsSelecting) return;

            // Move the FakeMenu along with the pointer while maintaining the offset
            Vector2 pointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out pointerPos);

            _rectTransform.anchoredPosition = pointerPos + _pointerOffset;
        }
    }
}
