using Map;
using Navigation;
using TargetsSystem.Points;
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
        
        public event OptionsList.OnOptionSelectedDel OptionSelected;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private MapControl MapControl => Global.Instance.UISetterV2.MapView.MapHandlePanel.MapControl;
        
        
        private void Awake()
        {
            _pathPointStateSetter = GetComponent<PathPointStateSetter>();
        }

        private void OnEnable()
        {
            _searchableDropDown.OptionSelected += NotifyOptionSelected;
            _searchableDropDown.OptionSelected += SetPathPoint;
        }

        private void OnDisable()
        {
            _searchableDropDown.OptionSelected -= NotifyOptionSelected;
            _searchableDropDown.OptionSelected -= SetPathPoint;
        }


        public void Initialize()
        {
            _searchableDropDown.Initialize(DataBase.GetPointsOptionInfos(true));
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

        private void NotifyOptionSelected(IOptionInfo optionInfo)
        {
            OptionSelected?.Invoke(optionInfo);
        }
        
        private void SetPathPoint(IOptionInfo optionInfo)
        {
            PointInfo pointInfo = optionInfo as PointInfo? ?? default;
            _pathPointStateSetter.SetState(pointInfo, FillingPathFieldType.Priority);
            FocusToPoint(pointInfo);
        }

        private void FocusToPoint(PointInfo pointInfo)
        {
            if (DataBase.TryGetPoint(pointInfo, out Point point))
                MapControl.GoToTarget(point.transform, pointInfo.Address.FloorIndex, false, true);
        }
    }
}
