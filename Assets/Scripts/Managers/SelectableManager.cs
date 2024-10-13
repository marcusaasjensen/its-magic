using System;
using System.Collections.Generic;
using System.Linq;
using Environment;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class SelectableManager : Utils.SceneSingleton<SelectableManager>
    {
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;
        [SerializeField] private GameObject selectableParent;
        
        private List<Selectable> _selectables = new();

        private void Start()
        {
            TouchInput.Instance.onSingleTouch.AddListener(CancelSelection);
        }
        
        private void CancelSelection()
        {
            foreach (var selectable in _selectables.Where(selectable => !selectable.IsDestroyed()))
            {
                selectable.IsSelected = false;
                selectable.transform.SetParent(null);
            }
            
            _selectables.Clear();
        }
        
        public void RegisterSelectable(Selectable selectable)
        {
            _selectables.Add(selectable);
            selectable.transform.SetParent(selectableParent.transform);
            onSelected.Invoke();
        }

        public void UnregisterSelectable(Selectable selectable)
        {
            _selectables.Remove(selectable);
            selectable.transform.SetParent(null);
            onDeselected.Invoke();
        }
    }
}