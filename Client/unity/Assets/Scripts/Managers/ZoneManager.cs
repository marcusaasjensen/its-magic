namespace Transition
{
    using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraView
    {
        public string name; 
        public CinemachineVirtualCamera camera;
        public bool showSwitchZoneButton; 
    }
    
    [System.Serializable]
    public class Zone
    {
        public string zoneName; 
        public CameraView[] views;
    }
    
    [SerializeField] private Zone[] zones; 
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button switchZoneButton;
    
    private int _currentZoneIndex = 0;
    private int _currentViewIndex = 0;
    
    private void Start()
    {
        Debug.Log("Adding listeners to buttons...");
        leftButton.onClick.AddListener(SwitchToLeftView);
        rightButton.onClick.AddListener(SwitchToRightView);
        switchZoneButton.onClick.AddListener(SwitchToNextZone);
    
        UpdateZoneAndView();
    }
    
    void SwitchToLeftView()
    {
        _currentViewIndex = (_currentViewIndex - 1 + zones[_currentZoneIndex].views.Length) % zones[_currentZoneIndex].views.Length;
        UpdateZoneAndView();
    }
    
    void SwitchToRightView()
    {
        _currentViewIndex = (_currentViewIndex + 1) % zones[_currentZoneIndex].views.Length;
        UpdateZoneAndView();
        Debug.Log("Switching to right view");
    }
    
     void SwitchToNextZone()
    {
        _currentZoneIndex = (_currentZoneIndex + 1) % zones.Length;
        _currentViewIndex = 0;
        UpdateZoneAndView();
    }
    
     void UpdateZoneAndView()
    {
        foreach (var zone in zones)
        {
            foreach (var view in zone.views)
            {
                view.camera.gameObject.SetActive(false);
            }
        }
    
        var currentView = zones[_currentZoneIndex].views[_currentViewIndex];
        currentView.camera.gameObject.SetActive(true);
    
        switchZoneButton.gameObject.SetActive(currentView.showSwitchZoneButton);
        
        Debug.Log($"Switched to view: {currentView.name} in zone {zones[_currentZoneIndex].zoneName}");
    }
}

}