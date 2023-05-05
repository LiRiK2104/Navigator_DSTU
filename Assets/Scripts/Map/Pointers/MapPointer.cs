using System;
using UnityEngine;

namespace Map
{
    public class MapPointer : Pointer
    {
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _pointAState;
        [SerializeField] private GameObject _pointBState;
        [SerializeField] private GameObject _transitStairsState;

        public bool Active =>
            _defaultState.activeSelf ||
            _pointAState.activeSelf ||
            _pointBState.activeSelf;
        
        public int? FloorIndex { get; private set; }

        public PointerState LastState { get; private set; } = PointerState.None;


        public void SetLastState()
        {
            if (LastState == PointerState.None || FloorIndex == null)
                return;
            
            SetState(LastState, FloorIndex.Value);
        }
        
        public void SetState(PointerState state, int floorIndex)
        {
            HideAll();

            switch (state)
            {
                case PointerState.Default:
                    _defaultState.SetActive(true);
                    break;
                
                case PointerState.PointA:
                    _pointAState.SetActive(true);
                    break;
                
                case PointerState.PointB:
                    _pointBState.SetActive(true);
                    break;
                
                case PointerState.TransitStairs:
                case PointerState.TransitElevator:
                    _transitStairsState.SetActive(true);
                    break;
                
                case PointerState.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            FloorIndex = floorIndex;
            LastState = state;
        }
        
        public void Hide(bool clearFloor = false) => HideAll(clearFloor);

        private void HideAll(bool clearFloor = false)
        {
            _defaultState.SetActive(false);
            _pointAState.SetActive(false);
            _pointBState.SetActive(false);
            _transitStairsState.SetActive(false);

            if (clearFloor)
                FloorIndex = null;
        }
    }

    public enum PointerState
    {
        None,
        Default,
        PointA,
        PointB,
        TransitStairs,
        TransitElevator
    }
}
