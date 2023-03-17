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
            if (TryGetNotActivePointer(out MapPointer pointer) == false)
                pointer = CreatePointer();

            pointer.SetState(request.PointerState);
            pointer.transform.position = request.TargetPosition;
        }

        public void DeactivateAllPointers()
        {
            foreach (var pointer in _spawnedPointers)
                pointer.Hide();
        }

        private bool TryGetNotActivePointer(out MapPointer foundPointer)
        {
            foundPointer = _spawnedPointers.FirstOrDefault(pointer => pointer.Active);
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
