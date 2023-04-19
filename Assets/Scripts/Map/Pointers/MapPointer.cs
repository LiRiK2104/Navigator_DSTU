using UnityEngine;

namespace Map
{
    public class MapPointer : Pointer
    {
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _pointAState;
        [SerializeField] private GameObject _pointBState;
        [SerializeField] private GameObject _transitStairsState;

        private PointerState _state;

        public bool Active =>
            _defaultState.activeSelf ||
            _pointAState.activeSelf ||
            _pointBState.activeSelf /*||
            _transitStairsState.activeSelf*/;

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
                    case PointerState.TransitElevator:
                        _transitStairsState.SetActive(true);
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
