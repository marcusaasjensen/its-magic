using Data;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private GlobalData globalData;
        [SerializeField] private bool resetDataOnStart;

        protected override void Awake()
        {
            base.Awake();
            
            if (resetDataOnStart)
            {
                globalData.ResetData();
            }
            
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        
    }
}