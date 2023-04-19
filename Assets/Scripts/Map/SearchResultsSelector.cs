using System;
using Navigation;
using TargetsSystem.Points;
using UI.FloorsSwitch;
using UI.Search;
using UI.Search.Options;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;

namespace Map
{
    public class SearchResultsSelector : MonoBehaviour
    {
        [SerializeField] private SearchableDropDown _searchableDropDown;
        
        private StateSetter StateSetter => Global.Instance.UISetterV2.StateSetter;
        private MapControl MapControl => Global.Instance.UISetterV2.MapHandlePanel.MapControl;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;
        private DataBase DataBase => Global.Instance.DataBase;


        private void OnEnable()
        {
            _searchableDropDown.OptionSelected += Select;
        }

        private void OnDisable()
        {
            _searchableDropDown.OptionSelected -= Select;
        }
        

        public void SetPointInfoState(PointInfo pointInfo)
        {
            StateSetter.SetState(StateType.PointInfo, out StateContainer state);
                    
            if (state.State is PointInfoState pointInfoState)
                pointInfoState.Initialize(pointInfo);
        }
        
        private void Select(IOptionInfo optionInfo)
        {
            switch (optionInfo)
            {
                case PointInfo pointInfo:
                    FloorsSwitcher.SwitchFloor(pointInfo.Address.FloorIndex);

                    if (DataBase.TryGetPoint(pointInfo, out Point point))
                        MapControl.GoToTarget(point.transform, false, true);
                    
                    SetPointInfoState(pointInfo);
                    break;
                
                case PointsGroup pointsGroup:
                    SetSearchResultsState(pointsGroup);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionInfo));
            }
        }

        private void SetSearchResultsState(PointsGroup pointsGroup)
        {
            StateSetter.SetState(StateType.SearchResults, out StateContainer state);
                    
            if (state.State is SearchResultsState searchResultsState)
                searchResultsState.Initialize(pointsGroup.Name, pointsGroup);
        }
    }
}
