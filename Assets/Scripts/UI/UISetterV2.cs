using System;
using Map;
using UI.SlidingPanel;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        [SerializeField] private UIStatesStorage _uiStatesStorage;
        [SerializeField] private StateSetter _stateSetter;
        [SerializeField] private SearchResultsSelector _searchResultsSelector;
        [SerializeField] private MapHandlePanel _mapHandlePanel;
        [SerializeField] private SlidingPanelHandler _slidingPanelHandler;

        
        public UIStatesStorage UIStatesStorage => _uiStatesStorage;
        public StateSetter StateSetter => _stateSetter;
        public SearchResultsSelector SearchResultsSelector => _searchResultsSelector;
        public MapHandlePanel MapHandlePanel => _mapHandlePanel;

        public SlidingPanelHandler SlidingPanelHandler => _slidingPanelHandler;


        public void Initialize()
        {
            if (_uiStatesStorage.TryGetState(StateType.SearchPanel, out SearchPanelState searchPanelState))
                searchPanelState.SearchPanelView.Initialize();
            
            if (_uiStatesStorage.TryGetState(StateType.PathSearch, out PathSearchState pathSearchState))
                pathSearchState.PathSearchView.Initialize();
            
            if (_uiStatesStorage.TryGetState(StateType.PathPlanning, out PathPlanningState pathPlanningState))
                pathPlanningState.InitializeView();

            InitializeSlidingPanel();
        }

        private void InitializeSlidingPanel()
        {
            Action<Transform> callback = targetPoint =>
            {
                if (_slidingPanelHandler.StatesStorage.TryGetState(targetPoint, out StateType stateType))
                    _stateSetter.SetState(stateType);
            };
            
            _slidingPanelHandler.Initialize(callback);
        }
    }
}
