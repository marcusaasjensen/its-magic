using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Managers
{
    public class ParallaxManager : MonoBehaviour
    {
        [SerializeField, Range(0f, 360f)]
        private float scrollAngle = 0f; // Global scroll angle control (0 to 360)
        [SerializeField] private float defaultRotationOffset = -90f; // Default rotation offset for all ParallaxElements

        [SerializeField] private float scrollSpeedMultiplier = 1f; // Speed adjustment via mouse scroll

        private List<ParallaxElement> _parallaxElements;

        void Start()
        {
            // Get all parallax elements in the scene and add them to the list
            _parallaxElements = new List<ParallaxElement>();
            foreach (var o in GameObject.FindGameObjectsWithTag("Parallax"))
            {
                var parallaxElement = o.GetComponent<ParallaxElement>();
                if (parallaxElement != null)
                {
                    _parallaxElements.Add(parallaxElement);
                }
            }
        }

        void Update()
        {
            // Adjust global scroll angle based on mouse input
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            scrollAngle += mouseScroll * scrollSpeedMultiplier * Time.deltaTime * 360f;

            // Wrap the scroll angle between 0 and 360
            scrollAngle = Mathf.Repeat(scrollAngle, 360f);

            // Update all ParallaxElements with the new scroll angle
            foreach (var element in _parallaxElements)
            {
                element.SetScrollAngle(scrollAngle + defaultRotationOffset);
            }
        }
        
        public void ChangeAngle(string message)
        {
            print(message);
            if(message == null)
            {
                return;
            }
            
            var fallingObjectMessage = JsonUtility.FromJson<MagicWandMessage>(message);
            if(fallingObjectMessage is not { type: "MagicWand" })
            {
                return;
            }
            
            scrollAngle = Mathf.Clamp(fallingObjectMessage.rotationInDegrees, 0, 360);
        }
    }
}