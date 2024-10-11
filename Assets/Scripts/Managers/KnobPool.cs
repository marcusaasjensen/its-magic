using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Managers
{
    public class KnobPool : MonoBehaviour
    {
        [SerializeField] private int knobCount;
        [SerializeField] private Knob knobPrefab;
        
        public List <Knob> Knobs { get; private set; }
        
        private void Awake()
        {
            Knobs = new List<Knob>();
            for (var i = 0; i < knobCount; i++)
            {
                var knob = Instantiate(knobPrefab, transform);
                knob.FingerId = i;
                Knobs.Add(knob);
            }
        }
    }
}