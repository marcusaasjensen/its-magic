using UnityEngine;

namespace Transition
{
    public class OpacityEffect : MonoBehaviour
    {
        public float opacitySpeed = 1.0f;
        public float opacityMin = 0.2f;
        public float opacityMax = 1.0f;

        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

       private void Update()
        {
            var alpha = Mathf.Lerp(opacityMin, opacityMax, (Mathf.Sin(Time.time * opacitySpeed) + 1) / 2);
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}