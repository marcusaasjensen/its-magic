using System;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Environment
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color selectionColor;
        
        public bool IsSelected { get; set; }
        
        private Color _defaultColor;
        
        private void Awake() => _defaultColor = spriteRenderer.color;

        private void Update()
        {
            OnSelection();
        }
        
        private void OnSelection()
        {
            if(TouchInput.Instance.Selection.IsPointInSelection(transform.position) && !IsSelected)
            {
                SetAsSelected(true);
            }
            
            spriteRenderer.color = IsSelected ? selectionColor : _defaultColor;
        }

        private void SetAsSelected(bool selected)
        {
            IsSelected = selected;
            if (selected)
            {
                SelectableManager.Instance.RegisterSelectable(this);
            }
            else
            {
                SelectableManager.Instance.UnregisterSelectable(this);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !IsSelected)
            {
                SetAsSelected(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetAsSelected(false);
            }
        }
    }
}