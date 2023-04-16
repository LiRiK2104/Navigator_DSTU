
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
        [SerializeField] private GameObject _transitElevatorState;

        private PointerState _state;

        public bool Active
        {
            get
            {
                return _defaultState.activeSelf ||
                       _pointAState.activeSelf ||
                       _pointBState.activeSelf ||
                       _transitStairsState.activeSelf ||
                       _transitElevatorState.activeSelf;
            }
        }

        public PointerState State
        {
            get => _state;
            set
            {
                HideAll();

                switch (value)
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
                        _transitStairsState.SetActive(true);
                        break;
                
                    case PointerState.TransitElevator:
                        _transitElevatorState.SetActive(true);
                        break;
                }

                _state = value;
            }
        }
        

        public void Hide() => HideAll();

        private void HideAll()
        {
            _defaultState.SetActive(false);
            _pointAState.SetActive(false);
            _pointBState.SetActive(false);
            _transitStairsState.SetActive(false);
            _transitElevatorState.SetActive(false);
        }
    }

    public enum PointerState
    {
        Default,
        PointA,
        PointB,
        TransitStairs,
        TransitElevator
    }
}
