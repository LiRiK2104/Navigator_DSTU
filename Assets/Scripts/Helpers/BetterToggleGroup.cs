using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    public class BetterToggleGroup : ToggleGroup 
    {
        public delegate void ChangedEventHandler(Toggle newActive);
        public event ChangedEventHandler ToggleChanged;
        public event Action Initialized;

        private bool _isInitialized;

        protected override void Start() 
        {
            base.Start();
            
            Initialize();
        }


        public void SelectToggle(Toggle targetToggle)
        {
            foreach (var toggle in m_Toggles)
            {
                if (toggle == targetToggle)
                    toggle.isOn = true;
            }
        }

        public bool TryGetIndex(Toggle toggle, out int index)
        {
            index = 0;
            
            for (int i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
        
        public bool TryGetToggle(int index, out Toggle toggle)
        {
            toggle = null;

            if (index < 0 || index >= m_Toggles.Count) 
                return false;
            
            toggle = m_Toggles[index];
            return true;
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;
            
            foreach (Transform transformToggle in gameObject.transform) 
            {
                var toggle = transformToggle.gameObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(isSelected => 
                {
                    if (isSelected == false) 
                        return;
                
                    var activeToggle = ActiveToggles().FirstOrDefault();
                    ToggleChanged?.Invoke(activeToggle);
                });
            }

            _isInitialized = true;
            Initialized?.Invoke();
        }
    }
}