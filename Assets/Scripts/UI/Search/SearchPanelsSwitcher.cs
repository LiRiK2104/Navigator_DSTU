using System;
using UI.Search.Options;
using UI.Search.States;
using UnityEngine;

namespace UI.Search
{
    [RequireComponent(typeof(SearchableDropDown))]
    public class SearchPanelsSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultPanel;
        [SerializeField] private SearchHistoryState _historyPanel;
        [SerializeField] private SearchState _searchPanel;

        private SearchableDropDown _searchableDropDown;

        private SearchHistoryWriter SearchHistoryWriter => Global.Instance.SearchHistoryWriter;
    

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
            bool hasHistory = SearchHistoryWriter.HasHistory;
            
            _searchPanel.gameObject.SetActive(false);
            _defaultPanel.SetActive(hasHistory == false);
            _historyPanel.gameObject.SetActive(hasHistory);
        }
    
        private void SetSearchPanel()
        {
            _historyPanel.gameObject.SetActive(false);
            _defaultPanel.SetActive(false);
            _searchPanel.gameObject.SetActive(true);
        }
    }
}
