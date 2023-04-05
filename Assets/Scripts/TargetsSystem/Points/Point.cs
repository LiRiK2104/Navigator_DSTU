using Map.Signs;
using Navigation;
using UnityEngine;

namespace TargetsSystem.Points
{
    public abstract class Point : MonoBehaviour
    {
        [SerializeField] private GraphwayNode _graphwayNode;
        [SerializeField] private SignCreator _signCreator;

        public SignCreator SignCreator => _signCreator;
        public Vector3 GraphwayNodePosition => _graphwayNode.transform.position;
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        private void Start()
        {
            Initialize();
        }
        
        private void OnDrawGizmos()
        {
            if (_graphwayNode != null)
            {
                var sphereRadius = 0.4f;
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, GraphwayNodePosition);
                Gizmos.DrawSphere(GraphwayNodePosition, sphereRadius);
            }
        }
        
        
        protected virtual void Initialize()
        {
            DataBase.TryGetPointInfo(this, out PointInfo pointInfo);
            _signCreator.Create(pointInfo);
        }
    }
}
