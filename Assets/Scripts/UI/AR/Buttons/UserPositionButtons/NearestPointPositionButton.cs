using System.Collections;
using Map;
using Navigation;
using TargetsSystem.Points;
using UnityEngine;

namespace UI.AR.Buttons.UserPositionButtons
{
    public class NearestPointPositionButton : UserPositionButton
    {
        private IEnumerator _findNearestPointRoutine;
        
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.MapView.SearchResultsSelector;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        private DataBase DataBase => Global.Instance.DataBase;

        
        private void OnDisable()
        {
            if (_findNearestPointRoutine != null) 
                StopCoroutine(_findNearestPointRoutine);
        }

        
        protected override void Action()
        {
            var userPathPoint = new PathPoint(UserPositionFinder.UserPosition, ARMain.UserFloorIndex);
            _findNearestPointRoutine = PathFinder.FindNearestPoint(userPathPoint, SearchResultsSelector.LastPointsGroup,
                SetPointInfoState);
            
            StartCoroutine(_findNearestPointRoutine);
        }

        private void SetPointInfoState(Point point)
        {
            if (DataBase.TryGetPointInfo(point, out PointInfo pointInfo))
                SearchResultsSelector.SetPointInfoState(pointInfo);
            
            _findNearestPointRoutine = null;
        }
    }
}
