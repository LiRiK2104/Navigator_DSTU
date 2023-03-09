using UnityEngine;

namespace Helpers
{
    public class FakeParenting : MonoBehaviour
    {
        [SerializeField] private Transform _fakeParent;
 
        private Vector3 _positionOffset;
        private Quaternion _rotationOffset;
 
        private void Start()
        {
            if(_fakeParent != null)
            {
                SetFakeParent(_fakeParent);
            }
        }
 
        private void Update()
        {
            if (_fakeParent == null)
                return;
 
            var targetPos = _fakeParent.position - _positionOffset;
            var targetRot = _fakeParent.localRotation * _rotationOffset;
 
            transform.position = RotatePointAroundPivot(targetPos, _fakeParent.position, targetRot);
            transform.localRotation = targetRot;
        }
 
        private void SetFakeParent(Transform parent)
        {
            _positionOffset = parent.position - transform.position;
            _rotationOffset = Quaternion.Inverse(parent.localRotation * transform.localRotation);
            _fakeParent = parent;
        }
 
        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 dir = point - pivot;
            dir = rotation * dir;
            point = dir + pivot;
            return point; 
        }
    }
}