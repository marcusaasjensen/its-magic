using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Player
{
    public class TouchInput : MonoBehaviourSingleton<TouchInput>
    {
        [SerializeField] private TextMeshProUGUI touchText;
        [SerializeField] private KnobSelection selection;
        [SerializeField] private DrawingSelector drawingSelection;
        
        public Touch[] Touches { get; private set; }
        public bool IsTouching => Touches.Length > 0;
        public KnobSelection Selection => selection;
        public DrawingSelector DrawingSelection => drawingSelection;

        public UnityEvent onSingleTouch;

        private bool _multipleTouches;
        
        private void Start() => Touches = Input.touches;
        
        private void Update()
        {
            Touches = Input.touches;
            UpdateTextDebug();
            OnSingleTouch();
        }
        
        private void OnSingleTouch()
        {
            switch (Touches.Length)
            {
                case > 1:
                    _multipleTouches = true;
                    break;
                case 1 when Touches[0].phase == TouchPhase.Ended && !_multipleTouches:
                    onSingleTouch.Invoke();
                    break;
                case 0:
                    _multipleTouches = false;
                    break;
            }
        }
        
        private void UpdateTextDebug()
        {
            if (touchText == null)
            {
                return;
            }

            touchText.text = "";

            foreach (var touch in Touches)
            {
                touchText.text += $"Touch {touch.fingerId}: Position {touch.position}, Phase {touch.phase}, Pressure {touch.pressure}\n";
            }
        }
    }
}