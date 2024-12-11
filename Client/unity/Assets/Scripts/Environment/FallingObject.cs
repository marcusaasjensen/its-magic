using UnityEngine;

namespace Environment
{
    public class FallingObject : Slashable
    {
        [SerializeField] private new string name;
        
        private Vector2 initialPosition;

        private void Awake()
        {
            initialPosition = transform.position;
        }
        
        public string Name => name;
        public Vector2 InitialPosition => initialPosition;
    }
}