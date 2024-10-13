using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Environment
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color selectionColor;
        public bool IsSelected { get; private set; }
        
        private Color _defaultColor;
        
        private void Awake() => _defaultColor = spriteRenderer.color;

        private void Update()
        {
            OnSelection();
        }
        
        private void OnSelection()
        {
            IsSelected = TouchInput.Instance.Selection.IsPointInSelection(transform.position);
            spriteRenderer.color = IsSelected ? selectionColor : _defaultColor;
            if (IsSelected)
            {
                print(gameObject.name);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                FollowPlayer(other.transform);
            }
        }
        
        private void FollowPlayer(Transform player)
        {
            transform.position = player.position;
        }
    }
}