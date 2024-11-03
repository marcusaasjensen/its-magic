using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class EdgeOpacityManager : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private float defaultOpacity = 0.15f;
        private float highlightOpacity = 1f;
        private float fadeSpeed = 1f;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            SetEdgeOpacity(defaultOpacity);
        }

        public void HighlightEdge()
        {
            SetEdgeOpacity(highlightOpacity);
            CancelInvoke("FadeEdgeOpacity");
            Invoke("FadeEdgeOpacity", 0.5f);
        }

        private void FadeEdgeOpacity()
        {
            StartCoroutine(FadeOpacityCoroutine());
        }

        private System.Collections.IEnumerator FadeOpacityCoroutine()
        {
            float startOpacity = _lineRenderer.startColor.a;
            float elapsedTime = 0;

            while (elapsedTime < fadeSpeed)
            {
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(startOpacity, defaultOpacity, elapsedTime / fadeSpeed);
                SetEdgeOpacity(newOpacity);
                yield return null;
            }

            SetEdgeOpacity(defaultOpacity);
        }

        private void SetEdgeOpacity(float opacity)
        {
            Color edgeColor = _lineRenderer.startColor;
            edgeColor.a = opacity;
            _lineRenderer.startColor = edgeColor;
            _lineRenderer.endColor = edgeColor;
        }
    }
}