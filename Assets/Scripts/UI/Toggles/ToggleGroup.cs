using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Toggles
{
    public class ToggleGroup : MonoBehaviour
    {
        private List<Toggle> _toggles = new List<Toggle>();

        public delegate void ChangedEventHandler(Toggle newActive);
        public event ChangedEventHandler ToggleChanged;


        public void Initialize()
        {
            _toggles = GetComponentsInChildren<Toggle>().ToList();

            foreach (var toggle in _toggles)
            {
                toggle.Initialize();
                
                toggle.ValueChanged.RemoveAllListeners();
                toggle.ValueChanged.AddListener(() => 
                {
                    SelectToggle(toggle);
                    ToggleChanged?.Invoke(toggle);
                });
            }
        }

        public bool TryGetActiveToggle(out Toggle foundToggle)
        {
            foundToggle = null;
            
            foreach (var toggle in _toggles.Where(toggle => toggle.IsOn))
            {
                foundToggle = toggle;
                return true;
            }

            return false;
        }

        public void SelectToggle(Toggle targetToggle)
        {
            foreach (var toggle in _toggles)
                toggle.IsOn = toggle == targetToggle;
        }

        public bool TryGetIndex(Toggle toggle, out int index)
        {
            index = 0;
            
            for (int i = 0; i < _toggles.Count; i++)
            {
                if (_toggles[i] == toggle)
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

            if (index < 0 || index >= _toggles.Count) 
                return false;
            
            toggle = _toggles[index];
            return true;
        }
    }
}