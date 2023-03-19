using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SearchPanel : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _targetsDropdown;
        [Space]
        [SerializeField] private List<Button> _closeButtons;

        public SearchableDropDown TargetsDropdown => _targetsDropdown;
        
        private void OnEnable()
        {
            _closeButtons.ForEach(button => button.onClick.AddListener(Close));
        }
        
        private void OnDisable()
        {
            _closeButtons.ForEach(button => button.onClick.RemoveListener(Close));
        }
        

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
