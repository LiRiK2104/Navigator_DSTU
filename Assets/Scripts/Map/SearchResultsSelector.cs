using System;
using Navigation;
using TargetsSystem.Points;
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


        private void OnEnable()
        {
            _searchableDropDown.OptionSelected += Select;
        }

        private void OnDisable()
        {
            _searchableDropDown.OptionSelected -= Select;
        }
        

        private void Select(IOptionInfo optionInfo)
        {
            switch (optionInfo)
            {
                case PointInfo pointInfo:
                    SetPointInfoState(pointInfo);
                    break;
                
                case PointsGroup pointsGroup:
                    SetSearchResultsState(pointsGroup);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(optionInfo));
            }
        }

        private void SetPointInfoState(PointInfo pointInfo)
        {
            StateSetter.SetState(StateType.PointInfo, out StateContainer state);
                    
            if (state.State is PointInfoState pointInfoState)
                pointInfoState.Initialize(pointInfo);
        }
        
        private void SetSearchResultsState(PointsGroup pointsGroup)
        {
            StateSetter.SetState(StateType.SearchResults, out StateContainer state);
                    
            if (state.State is SearchResultsState searchResultsState)
                searchResultsState.Initialize(pointsGroup.Name, pointsGroup);
        }
    }
}
