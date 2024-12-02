using System;
using Cinemachine;
using UnityEngine;
using Utils;

namespace Managers
{
    public class CameraManager : SceneSingleton<CameraManager>
    {
        [SerializeField] private CinemachineVirtualCamera currentCamera;
        public event Action OnCameraSwitch;

        public CinemachineVirtualCamera CurrentCamera => currentCamera;
        
        public Camera MainCamera { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            MainCamera = Camera.main;
        }
        
        public void SwitchToCamera(CinemachineVirtualCamera newCamera)
        {
            currentCamera.gameObject.SetActive(false);
            currentCamera = newCamera;
            currentCamera.gameObject.SetActive(true);
            MainCamera = Camera.main;
            OnCameraSwitch?.Invoke();
        }
    }
}