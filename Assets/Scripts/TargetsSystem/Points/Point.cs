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
        public GraphwayNode GraphwayNode
        {
            get => _graphwayNode;
            set => _graphwayNode = value;
        }
        public Vector3 GraphwayNodePosition => _graphwayNode.transform.position;
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public virtual void Initialize()
        {
            if (TryGetInfo(out PointInfo pointInfo))
                _signCreator.Create(pointInfo);
        }

        public bool TryGetInfo(out PointInfo pointInfo)
        {
            return DataBase.TryGetPointInfo(this, out pointInfo);
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
    }
}
