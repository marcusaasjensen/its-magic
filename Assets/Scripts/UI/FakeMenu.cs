using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Player
{
    public class FakeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject fakeMenu;
        [SerializeField] private Animator animator;
        [SerializeField] private List<FakeMenuItem> menuItems;

        private Camera _camera;
        
        private int _counter;
        private static readonly int IsOdd = Animator.StringToHash("isOdd");

        private void Awake()
        {
            _camera = Camera.main;
        }
        
        public void EnabledAllMenuItems()
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.IsDraggable = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleMenu();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ToggleRotation();
            }
            
            //FollowMouse();
        }

        private void ToggleMenu()
        {
            fakeMenu.SetActive(!fakeMenu.activeSelf);
        }

        private void ToggleRotation()
        {
            animator.SetBool(IsOdd, _counter % 2 == 0);
            _counter++;
        }

        private void FollowMouse()
        {
            var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
        
    }
}