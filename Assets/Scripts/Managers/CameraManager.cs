using Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera currentCamera;
        
        public void SwitchToCamera(CinemachineVirtualCamera newCamera)
        {
            currentCamera.gameObject.SetActive(false);
            currentCamera = newCamera;
            currentCamera.gameObject.SetActive(true);
        }
    }
}