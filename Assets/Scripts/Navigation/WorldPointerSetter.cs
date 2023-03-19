using UnityEngine;

namespace Navigation
{
    public class WorldPointerSetter : PointerSetter
    {
        [SerializeField] private Pointer _pointerPrefab;
        
        private Pointer _pointer;


        protected override void SetPointer(Vector3 targetPosition)
        {
            if (_pointer == null)
                _pointer = Instantiate(_pointerPrefab, Environment.transform);

            _pointer.transform.position = targetPosition;
        }
    }
}
