using System;
using System.Linq;
using Map;
using Navigation;
using TargetsSystem.Points;
using TMPro;
using UI.Search.Options;
using UI.StateSystem.Groups;
using UI.StateSystem.Setters;
using UnityEngine;
using Toggle = UI.Toggles.Toggle;

namespace UI.FloorsSwitch
{
    [RequireComponent(typeof(Toggle))]
    public class FloorToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _number;
        [SerializeField] private GameObject _pointAMark;
        [SerializeField] private GameObject _pointBMark;
        [SerializeField] private GameObject _selectedPointMark;

        private int _floorIndex;

        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.MapView.SearchResultsSelector;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;
        private DataBase DataBase => Global.Instance.DataBase;


        private void OnEnable()
        {
            UpdatePathPointAMark(PathFinder.PointA);
            UpdatePathPointBMark(PathFinder.PointB);
            UpdateSearchResultsMark();
            
            PathFinder.PointASet += UpdatePathPointAMark;
            PathFinder.PointBSet += UpdatePathPointBMark;
            PathFinder.PointARemoved += HidePathPointAMark;
            PathFinder.PointBRemoved += HidePathPointBMark;
            SearchResultsSelector.OptionSelected += UpdateSearchResultOption;
            StateSetter.GroupClosed += HideSearchResultMark;
        }

        private void OnDisable()
        {
            PathFinder.PointASet -= UpdatePathPointAMark;
            PathFinder.PointBSet -= UpdatePathPointBMark;
            PathFinder.PointARemoved -= HidePathPointAMark;
            PathFinder.PointBRemoved -= HidePathPointBMark;
            SearchResultsSelector.OptionSelected -= UpdateSearchResultOption;
            StateSetter.GroupClosed -= HideSearchResultMark;
        }


        public void Initialize(int floorIndex, int number)
        {
            _floorIndex = floorIndex;
            _number.text = number.ToString();
            HideAllMarks();
        }

        private void UpdatePathPointAMark(PathPoint pointA)
        {
            if (pointA == null)
                HidePathPointAMark();
            else
                UpdatePathPointAMark(pointA.FloorIndex);
        }
        
        private void UpdatePathPointBMark(PathPoint pointB)
        {
            if (pointB == null)
                HidePathPointBMark();
            else
                UpdatePathPointBMark(pointB.FloorIndex);
        }
        
        private void UpdateSearchResultsMark()
        {
            bool show = false;
            
            foreach (var block in DataBase.Floors[_floorIndex].Blocks)
            {
                foreach (var point in block.Points)
                {
                    var sign = point.SignCreator.Sign;
                    
                    if (sign != null)
                        show = show || sign.Selected;
                }
            }
            
            UpdateMark(FloorToggleMark.SearchResult, show);
        }
        
        
        private void UpdatePathPointAMark(int floorIndex)
        {
            bool show = floorIndex == _floorIndex;
            UpdatePathPointAMark(show);
        }
        
        private void UpdatePathPointAMark(bool show)
        {
            UpdateMark(FloorToggleMark.PointA, show);
        }
        
        private void UpdatePathPointBMark(int floorIndex)
        {
            bool show = floorIndex == _floorIndex;
            UpdatePathPointBMark(show);
        }
        
        private void UpdatePathPointBMark(bool show)
        {
            UpdateMark(FloorToggleMark.PointB, show);
        }
        
        private void HidePathPointAMark()
        {
            UpdatePathPointAMark(false);
        }
        
        private void HidePathPointBMark()
        {
            UpdatePathPointBMark(false);
        }

        private void HideAllMarks()
        {
            HideSearchResultMark();
            HidePathPointsMarks();
        }
        
        private void HideSearchResultMark(StatesGroup group = null)
        {
            if (group == null || group is SearchGroup)
                _selectedPointMark.SetActive(false);
        }

        private void HidePathPointsMarks()
        {
            _pointAMark.SetActive(false);
            _pointBMark.SetActive(false);
        }

        private void UpdateSearchResultOption(IOptionInfo optionInfo)
        {
            if (optionInfo is not PointsGroup pointsGroup)
                return;

            int[] floorsIndexes = pointsGroup.GetFloorsIndexes();
            bool show = floorsIndexes.Contains(_floorIndex);
            UpdateMark(FloorToggleMark.SearchResult, show);
        }

        private void UpdateMark(FloorToggleMark mark, bool show)
        {
            switch (mark)
            {
                case FloorToggleMark.SearchResult:
                    _selectedPointMark.SetActive(show);
                    break;
                
                case FloorToggleMark.PointA:
                    _pointAMark.SetActive(show);
                    break;
                
                case FloorToggleMark.PointB:
                    _pointBMark.SetActive(show);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(mark), mark, null);
            }
        }
    }

    public enum FloorToggleMark
    {
        SearchResult,
        PointA,
        PointB
    }
}
