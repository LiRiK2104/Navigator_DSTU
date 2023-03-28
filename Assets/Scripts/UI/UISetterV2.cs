using Map;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        [SerializeField] private UIStatesStorage _uiStatesStorage;
        [SerializeField] private StateSetter _stateSetter;
        [SerializeField] private SearchResultsSelector _searchResultsSelector;
        [SerializeField] private MapHandlePanel _mapHandlePanel;

        
        public UIStatesStorage UIStatesStorage => _uiStatesStorage;
        public StateSetter StateSetter => _stateSetter;
        public SearchResultsSelector SearchResultsSelector => _searchResultsSelector;
        public MapHandlePanel MapHandlePanel => _mapHandlePanel;
    }
}
