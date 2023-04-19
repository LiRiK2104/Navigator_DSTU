using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Toggles
{
    [RequireComponent(typeof(Button))]
    public class Toggle : MonoBehaviour
    {
        [SerializeField] private GameObject _selection;

        private bool _isOn;

        public ToggleClickedEvent ValueChanged;
        
        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                _selection.gameObject.SetActive(value);
            }
        }

        
        public void Initialize()
        {
            var button = GetComponent<Button>();
            ValueChanged = new ToggleClickedEvent();
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(NotifyValueChanged);
        }

        private void NotifyValueChanged()
        {
            ValueChanged.Invoke();
        }
    }

    public class ToggleClickedEvent : UnityEvent {}
}
