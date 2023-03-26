using Map.Signs.States;
using TargetsSystem.Points;
using TargetsSystem.Rooms;
using UnityEngine;

namespace Map.Signs
{
    public class Sign : MonoBehaviour
    {
        [SerializeField] private DefaultState _defaultState;
        [SerializeField] private SearchResultState _searchResultState;

        
        public void Initialize(SignPreset signPreset, AccessibleRoom room = null)
        {
            _defaultState.Initialize(signPreset, room);
            _searchResultState.Initialize(signPreset, room);
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
