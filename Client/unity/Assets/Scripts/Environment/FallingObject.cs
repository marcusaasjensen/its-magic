using UnityEngine;

namespace Environment
{
    public class FallingObject : Slashable
    {
        [SerializeField] private new string name;
        
        private Vector2 _initialPosition;

        private void Awake()
        {
            _initialPosition = transform.position;
        }
        
        public string Name => name;
        public Vector2 InitialPosition => _initialPosition;
    }
}