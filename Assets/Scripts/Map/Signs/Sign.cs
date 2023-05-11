using Map.Signs.States;
using Navigation;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEngine;
using DefaultState = Map.Signs.States.DefaultState;

namespace Map.Signs
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] private GameObject _view;
        [SerializeField] private DefaultState _defaultState;
        [SerializeField] private SearchResultState _searchResultState;
        [SerializeField] private SignCollider _signCollider;

        private PointInfo _pointInfo;
        
        public bool Selected { get; private set; }
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;


        private void OnEnable()
        {
            StateSetter.StateSet += HandleUIState;
        }

        private void OnDisable()
        {
            StateSetter.StateSet -= HandleUIState;
        }


        public void Initialize(PointInfo pointInfo, SignPreset signPreset)
        {
            _pointInfo = pointInfo;
            _defaultState.Initialize(signPreset, pointInfo);
            _searchResultState.Initialize(pointInfo);
            _signCollider.Initialize(pointInfo);
            Deselect();
        }

        public void Select()
        {
            _defaultState.gameObject.SetActive(false);
            _searchResultState.gameObject.SetActive(true);
            Selected = true;
        }
        
        public void Deselect()
        {
            _defaultState.gameObject.SetActive(true);
            _searchResultState.gameObject.SetActive(false);
            Selected = false;
        }

        private void HandleUIState(StateType stateType)
        {
            bool visible = true;

            if (stateType == StateType.PathPointInfo)
                visible = _pointInfo.IsWayPoint;

            _view.SetActive(visible);
        }
    }
}
