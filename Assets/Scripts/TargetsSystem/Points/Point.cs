using Map.Signs;
using Navigation;
using UnityEngine;

namespace TargetsSystem.Points
{
    public abstract class Point : MonoBehaviour
    {
        [SerializeField] private TargetPoint _targetPoint;
        [SerializeField] private SignCreator _signCreator;

        public SignCreator SignCreator => _signCreator;
        public Vector3 TargetPointPosition => _targetPoint.transform.position;
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        private void Start()
        {
            Initialize();
        }
        
        
        protected virtual void Initialize()
        {
            DataBase.TryGetPointInfo(this, out PointInfo pointInfo);
            _signCreator.Create(pointInfo);
        }
    }
}
