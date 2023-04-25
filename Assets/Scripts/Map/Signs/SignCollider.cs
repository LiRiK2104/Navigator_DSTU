using Navigation;
using UI.StateSystem;
using UI.StateSystem.Groups;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map.Signs
{
    public class SignCollider : MonoBehaviour
    {
        private PointInfo _pointInfo;
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.MapView.SearchResultsSelector;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;
        

        public void Initialize(PointInfo pointInfo)
        {
            _pointInfo = pointInfo;
        }

        public void SetPointInfoState()
        {
            if (StateSetter.CurrentGroup is NavigationGroup navigationGroup)
            {
                StateSetter.SetState(StateType.PathPointInfo, out StateContainer stateContainer);

                if (stateContainer.State is PathPointInfoState pathPointInfoState)
                    pathPointInfoState.Initialize(_pointInfo);
            }
            else
            {
                SearchResultsSelector.SetPointInfoState(_pointInfo);   
            }
        }
    }
}
