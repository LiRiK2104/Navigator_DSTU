using Map.Signs;
using UnityEngine;

namespace TargetsSystem.Points
{
    public abstract class Point : MonoBehaviour
    {
        [SerializeField] private TargetPoint _targetPoint;
        [SerializeField] private SignCreator _signCreator;

        public SignCreator SignCreator => _signCreator;
        public Vector3 TargetPointPosition => _targetPoint.transform.position;
        
        
        private void Start()
        {
            Initialize();
        }
        
        
        protected virtual void Initialize()
        {
            _signCreator.Create();
        }
    }
}
