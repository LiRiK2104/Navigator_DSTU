using UI.Search;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class SearchPanelState : State
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;


        public override void OnOpen()
        {
            _searchableDropDown.InputFieldIsActive = true;
        }

        public override void OnClose()
        {
            _searchableDropDown.InputFieldValue = string.Empty;
            _searchableDropDown.Reset();
            _searchableDropDown.InputFieldIsActive = false;
        }
    }
}
