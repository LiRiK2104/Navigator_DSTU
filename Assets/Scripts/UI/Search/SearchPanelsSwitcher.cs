using System;
using UnityEngine;

namespace UI.Search
{
    [RequireComponent(typeof(Search.SearchableDropDown))]
    public class SearchPanelsSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultPanel;
        [SerializeField] private GameObject _searchPanel;

        private Search.SearchableDropDown _searchableDropDown;
    

        private void Awake()
        {
            _searchableDropDown = GetComponent<Search.SearchableDropDown>();
            SetDefaultPanel();
        }

        private void OnEnable()
        {
            _searchableDropDown.ValueChanged += SwitchPanel;
        }

        private void OnDisable()
        {
            _searchableDropDown.ValueChanged -= SwitchPanel;
        }

    
        private void SwitchPanel(string searchArg)
        {
            if (searchArg == String.Empty)
                SetDefaultPanel();
            else
                SetSearchPanel();
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
