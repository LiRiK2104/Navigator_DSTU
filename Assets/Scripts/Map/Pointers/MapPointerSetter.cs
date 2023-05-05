using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using UI.FloorsSwitch;
using UnityEngine;

namespace UI
{
    public class MapPointerSetter : MonoBehaviour
    {
        [SerializeField] private MapPointer _pointerPrefab;

        private List<MapPointer> _spawnedPointers = new List<MapPointer>();
        
        private AREnvironment Environment => Global.Instance.ArEnvironment;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;


        private void OnEnable()
        {
            FloorsSwitcher.FloorSwitched += UpdatePointers;
        }

        private void OnDisable()
        {
            FloorsSwitcher.FloorSwitched -= UpdatePointers;
        }
        
        
        public void SetPointer(PointerSetRequest request)
        {
            if (TryGetPointer(request, out MapPointer pointer) == false)
                pointer = CreatePointer();

            pointer.SetState(request.PointerState, FloorsSwitcher.CurrentFloorIndex);
            pointer.transform.position = request.TargetPosition;
        }

        public void HidePointers(bool clearFloor, params PointerState[] states)
        {
            foreach (var pointer in _spawnedPointers)
            {
                if (states.Contains(pointer.LastState))
                    pointer.Hide(clearFloor);
            }
        }
        
        public void DeactivateAllPointers()
        {
            var states = (PointerState[])Enum.GetValues(typeof(PointerState));
            HidePointers(true, states);
        }
        
        public void UpdatePointers(int currentFloor)
        {
            _spawnedPointers.ForEach(pointer => UpdatePointer(pointer, currentFloor));
        }

        private bool TryGetPointer(PointerSetRequest request, out MapPointer foundPointer)
        {
            foundPointer = _spawnedPointers.FirstOrDefault(pointer => pointer.LastState == request.PointerState);
            return foundPointer != null;
        }

        private MapPointer CreatePointer()
        {
            var pointer = Instantiate(_pointerPrefab, Environment.transform);
            _spawnedPointers.Add(pointer);

            return pointer;
        }

        private void UpdatePointer(MapPointer pointer, int currentFloor)
        {
            if (pointer.FloorIndex.HasValue && pointer.FloorIndex.Value == currentFloor)
                pointer.SetLastState();
            else
                pointer.Hide();
        }
    }

    public struct PointerSetRequest
    {
        public Vector3 TargetPosition { get; }
        public PointerState PointerState { get; }

        
        public PointerSetRequest(Vector3 targetPosition, PointerState pointerState)
        {
            TargetPosition = targetPosition;
            PointerState = pointerState;
        }
    }
}
