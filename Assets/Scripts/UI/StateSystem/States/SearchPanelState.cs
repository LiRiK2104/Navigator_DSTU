using System;
using UI.Search;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class SearchPanelState : State
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        
        public override void Initialize()
        {
            _searchableDropDown.InputFieldIsActive = true;
        }

        public override void OnClose()
        {
            _searchableDropDown.InputFieldValue = String.Empty;
            _searchableDropDown.Reset();
            _searchableDropDown.InputFieldIsActive = false;
        }
        
        public void Initialize(string input)
        {
            _searchableDropDown.InputFieldValue = input;
            Initialize();
        }
    }
}
