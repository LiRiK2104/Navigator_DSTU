using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Navigation;
using UnityEngine;

namespace UI
{
    public class MapPointerSetter : MonoBehaviour
    {
        [SerializeField] private MapPointer _pointerPrefab;

        private List<MapPointer> _spawnedPointers = new List<MapPointer>();
        
        private AREnvironment Environment => Global.Instance.ArEnvironment;

        
        public void SetPointers(List<PointerSetRequest> requests)
        {
            foreach (var request in requests)
                SetPointer(request);
        }

        public void SetPointer(PointerSetRequest request)
        {
            if (TryGetPointer(request, out MapPointer pointer) == false)
                pointer = CreatePointer();

            pointer.State = request.PointerState;
            pointer.transform.position = request.TargetPosition;
        }

        public void DeactivatePointers(params PointerState[] states)
        {
            foreach (var pointer in _spawnedPointers)
            {
                if (states.Contains(pointer.State))
                    pointer.Hide();
            }
        }
        
        public void DeactivateAllPointers()
        {
            var states = (PointerState[])Enum.GetValues(typeof(PointerState));
            DeactivatePointers(states);
        }

        private bool TryGetPointer(PointerSetRequest request, out MapPointer foundPointer)
        {
            foundPointer = _spawnedPointers.FirstOrDefault(pointer => pointer.State == request.PointerState);
            return foundPointer != null;
        }

        private MapPointer CreatePointer()
        {
            var pointer = Instantiate(_pointerPrefab, Environment.transform);
            _spawnedPointers.Add(pointer);

            return pointer;
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
