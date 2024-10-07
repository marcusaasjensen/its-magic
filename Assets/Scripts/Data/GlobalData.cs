using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GlobalData", menuName = "Data/GlobalData")]
    public class GlobalData : ScriptableObject
    {
        public bool resetData;
        public List<string> collectedItemKeys;
        
        private void OnValidate()
        {
        }
    }
}