using UI.Views;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class SearchPanelState : State
    {
        [SerializeField] private SearchPanelView _searchPanelView;

        public SearchPanelView SearchPanelView => _searchPanelView;
        

        public override void OnOpen()
        {
            _searchPanelView.Activate();
        }

        public override void OnClose()
        {
            _searchPanelView.Deactivate();
        }
    }
}
