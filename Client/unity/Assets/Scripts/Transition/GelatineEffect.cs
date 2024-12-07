using UnityEngine;

namespace Transition
{
    public class GelatineEffect : MonoBehaviour
    {
        public float bounceSpeed = 2.0f;     
        public float stretchFactor = 0.2f;
    
        private Vector3 _originalScale;
    
        private void Start()
        {
            _originalScale = transform.localScale;
        }
    
        private void Update()
        {
            var stretch = Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed));
            transform.localScale = new Vector3(
                _originalScale.x + stretch * -stretchFactor, 
                _originalScale.y + stretch * stretchFactor,
                _originalScale.z
            );
        }
    }
}