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
        private Vector2 _pointerOffset;
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private Vector2 _screenCenter;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
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
            
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            var menuPosition = _rectTransform.position;
            
            Vector2 directionFromCenter = menuPosition - (Vector3)_screenCenter;

            var angle = Mathf.Atan2(directionFromCenter.y, directionFromCenter.x) * Mathf.Rad2Deg;

            _rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90 + 180);
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

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out var pointerPos);

            _rectTransform.anchoredPosition = pointerPos + _pointerOffset;
        }
    }
}
