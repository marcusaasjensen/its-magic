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
            OnResetData();
        }

        private void OnResetData()
        {
            if (resetData)
            {
                ResetData();
            }
        }

        public void ResetData()
        {
            collectedItemKeys.Clear();
            resetData = false;
        }
    }
}