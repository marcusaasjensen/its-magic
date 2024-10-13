using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Player
{
    public class TouchInput : MonoBehaviourSingleton<TouchInput>
    {
        [SerializeField] private TextMeshProUGUI touchText;
        [SerializeField] private KnobSelection selection;
        
        public Touch[] Touches { get; private set; }
        public bool IsTouching => Touches.Length > 0;
        public KnobSelection Selection => selection;
        
        private void Start() => Touches = Input.touches;
        
        private void Update()
        {
            Touches = Input.touches;
            UpdateTextDebug();
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