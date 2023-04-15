using Navigation;
using UI.Search;
using UI.Search.Options;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(PathPointStateSetter))]
    public class PathSearchView : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        
        private PathPointStateSetter _pathPointStateSetter;

        public SearchableDropDown SearchableDropDown => _searchableDropDown;
        
        
        private void Awake()
        {
            _pathPointStateSetter = GetComponent<PathPointStateSetter>();
        }

        private void OnEnable()
        {
            _searchableDropDown.OptionSelected += SetPathPoint;
        }

        private void OnDisable()
        {
            _searchableDropDown.OptionSelected -= SetPathPoint;
        }

        
        private void SetPathPoint(IOptionInfo optionInfo)
        {
            PointInfo pointInfo = optionInfo as PointInfo? ?? default;
            _pathPointStateSetter.SetState(pointInfo, FillingPathFieldType.Priority);
        }
    }
}
