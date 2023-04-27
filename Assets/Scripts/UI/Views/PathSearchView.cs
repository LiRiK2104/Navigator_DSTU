using Navigation;
using UI.Search;
using UI.Search.Options;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;

namespace UI.Views
{
    [RequireComponent(typeof(PathPointStateSetter))]
    public class PathSearchView : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        
        private PathPointStateSetter _pathPointStateSetter;
        
        private DataBase DataBase => Global.Instance.DataBase;
        
        
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


        public void Initialize()
        {
            _searchableDropDown.Initialize(DataBase.GetPointsOptionInfos());
        }
        
        public void Activate()
        {
            _searchableDropDown.InputFieldIsActive = true;
        }
        
        public void Deactivate()
        {
            _searchableDropDown.InputFieldValue = string.Empty;
            _searchableDropDown.Reset();
            _searchableDropDown.InputFieldIsActive = false;
        }
        
        private void SetPathPoint(IOptionInfo optionInfo)
        {
            PointInfo pointInfo = optionInfo as PointInfo? ?? default;
            _pathPointStateSetter.SetState(pointInfo, FillingPathFieldType.Priority);
        }
    }
}