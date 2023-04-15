using UI.Search;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathSearchState : State
    {
        [SerializeField] private PathSearchView pathSearchView;
        
        private SearchableDropDown SearchableDropDown => pathSearchView.SearchableDropDown;

        
        public override void OnOpen()
        {
            SearchableDropDown.InputFieldIsActive = true;
        }

        public override void OnClose()
        {
            SearchableDropDown.InputFieldValue = string.Empty;
            SearchableDropDown.Reset();
            SearchableDropDown.InputFieldIsActive = false;
        }
    }
}
