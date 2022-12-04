using Navigation;
using UnityEngine;

namespace UI
{
    public class MapPointerSetter : PointerSetter
    {
        [SerializeField] private MinimapPointer _pointerPrefab;

        private MinimapPointer _pointer;

        private Camera MinimapCamera => Global.Instance.CameraContainer.MinimapCamera;


        protected override void SetPointer(TargetPoint targetPoint)
        {
            if (_pointer == null)
            {
                _pointer = Instantiate(_pointerPrefab, Environment.transform);
                _pointer.Initialize(MinimapCamera);
            }

            _pointer.transform.position = targetPoint.Transform.position;
        }
    }
}
