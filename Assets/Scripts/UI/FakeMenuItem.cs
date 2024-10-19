﻿using System;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class FakeMenuItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private UnityEvent onItemReleased;
        [SerializeField] private Color disabledColor;
        [SerializeField] private Color confirmColor;
        [SerializeField] private float resetSpeed = 5f;
        [SerializeField] private float confirmDistance = 200f;
        [SerializeField] private bool isDraggableOnStart = true;

        public bool IsDraggable { get; set; } = true;

        private Image _image;
        private Color _defaultColor;
        private Vector3 _defaultPosition;
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;

        private bool _isBeingDragged;
        private Vector2 _pointerOffset;
        private bool _isResetting;
        private bool _canConfirmUse;

        private Vector3 _menuCenter;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            _defaultPosition = _rectTransform.anchoredPosition;
            _defaultColor = _image.color;
            _parentCanvas = GetComponentInParent<Canvas>();
            IsDraggable = isDraggableOnStart;
            _menuCenter = _defaultPosition;
        }

        private void Update()
        {
            UpdateUI();

            if (_isResetting)
            {
                _rectTransform.anchoredPosition = Vector3.Lerp(_rectTransform.anchoredPosition, _defaultPosition, Time.deltaTime * resetSpeed);
                if (Vector3.Distance(_rectTransform.anchoredPosition, _defaultPosition) < 0.1f)
                {
                    _rectTransform.anchoredPosition = _defaultPosition;
                    _isResetting = false;
                }
            }
        }

        private void UpdateUI()
        {
            if (_isBeingDragged)
            {
                return;
            }
            _image.color = IsDraggable ? _defaultColor : disabledColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsDraggable || TouchInput.Instance.Selection.IsSelecting) return;

            _isBeingDragged = true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out Vector2 localPointerPos);

            _pointerOffset = _rectTransform.anchoredPosition - localPointerPos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsDraggable || TouchInput.Instance.Selection.IsSelecting) return;

            _isBeingDragged = false;

            if (_canConfirmUse)
            {
                onItemReleased.Invoke();
                IsDraggable = false;
            }

            ResetItem();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDraggable || !_isBeingDragged || TouchInput.Instance.Selection.IsSelecting) return;

            Vector2 pointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position,
                _parentCanvas.worldCamera,
                out pointerPos);

            _rectTransform.anchoredPosition = pointerPos + _pointerOffset;

            float distanceFromCenter = Vector3.Distance(_rectTransform.anchoredPosition, _menuCenter);

            if (distanceFromCenter >= confirmDistance)
            {
                _image.color = confirmColor;
                _canConfirmUse = true;
            }
            else
            {
                _image.color = _defaultColor;
                _canConfirmUse = false;
            }
        }

        private void ResetItem()
        {
            _isResetting = true;
            _canConfirmUse = false;
        }
    }
}
