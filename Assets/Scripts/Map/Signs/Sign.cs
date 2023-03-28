using Map.Signs.States;
using Navigation;
using TargetsSystem.Points;
using UnityEngine;
using DefaultState = Map.Signs.States.DefaultState;

namespace Map.Signs
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] private DefaultState _defaultState;
        [SerializeField] private SearchResultState _searchResultState;
        [SerializeField] private SignCollider _signCollider;

        
        public void Initialize(PointInfo pointInfo, SignPreset signPreset, AccessibleRoom room = null)
        {
            _defaultState.Initialize(signPreset, room);
            _searchResultState.Initialize(signPreset, room);
            _signCollider.Initialize(pointInfo);
            Deselect();
        }

        public void Select()
        {
            _defaultState.gameObject.SetActive(false);
            _searchResultState.gameObject.SetActive(true);
        }
        
        public void Deselect()
        {
            _defaultState.gameObject.SetActive(true);
            _searchResultState.gameObject.SetActive(false);
        }
    }
}
