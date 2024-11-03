using System;
using Managers;
using Player;
using UnityEngine;

namespace Environment
{
    public class Selectable : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color selectionColor;
        
        public bool IsSelected { get; set; }
        
        private bool _isTriggered;
        
        private Color _defaultColor;
        
        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultColor = _spriteRenderer.color;
        }

        private void Update()
        {
            OnSelection();
        }
        
        private void OnSelection()
        {
            if((TouchInput.Instance.Selection.IsPointInSelection(transform.position) || TouchInput.Instance.DrawingSelection.IsPointInSelection(transform.position)) && !IsSelected && !_isTriggered)
            {
                SetAsSelected(true);
            }

            if (!(TouchInput.Instance.Selection.IsPointInSelection(transform.position) || TouchInput.Instance.DrawingSelection.IsPointInSelection(transform.position)) && IsSelected && !TouchInput.Instance.IsTouching)
            {
                SetAsSelected(false);
            }
            
            _spriteRenderer.color = IsSelected ? selectionColor : _defaultColor;
        }

        private void SetAsSelected(bool selected)
        {
            IsSelected = selected;
            SelectableManager.Instance.RegisterSelectable(this, selected);
        }

        private void OnDestroy()
        {
            SelectableManager.Instance.RegisterSelectable(this, false);
        }

        private void OnDisable()
        {
            SelectableManager.Instance.RegisterSelectable(this, false);
        }

        /*private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !IsSelected)
            {
                SetAsSelected(true);
                _isTriggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetAsSelected(false);
                _isTriggered = false;
            }
        }*/
    }
}