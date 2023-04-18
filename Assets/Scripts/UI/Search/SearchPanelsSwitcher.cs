using System;
using UI.Search.Options;
using UnityEngine;

namespace UI.Search
{
    [RequireComponent(typeof(SearchableDropDown))]
    public class SearchPanelsSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultPanel;
        [SerializeField] private GameObject _searchPanel;

        private SearchableDropDown _searchableDropDown;
    

        private void Awake()
        {
            _searchableDropDown = GetComponent<SearchableDropDown>();
            SetDefaultPanel();
        }

        private void OnEnable()
        {
            _searchableDropDown.ValueChanged += SwitchPanel;
            _searchableDropDown.OptionSelected += SetDefaultPanel;
        }

        private void OnDisable()
        {
            _searchableDropDown.ValueChanged -= SwitchPanel;
            _searchableDropDown.OptionSelected -= SetDefaultPanel;
        }

    
        private void SwitchPanel(string searchArg)
        {
            if (searchArg == String.Empty)
                SetDefaultPanel();
            else
                SetSearchPanel();
        }

        private void SetDefaultPanel(IOptionInfo optionInfo)
        {
            SetDefaultPanel();
        }
        
        private void SetDefaultPanel()
        {
            _defaultPanel.SetActive(true);
            _searchPanel.SetActive(false);
        }
    
        private void SetSearchPanel()
        {
            _defaultPanel.SetActive(false);
            _searchPanel.SetActive(true);
        }
    }
}
