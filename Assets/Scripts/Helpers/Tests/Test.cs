using System.Linq;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using UnityEngine;

namespace Helpers.Tests
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private LineRenderer _lineRenderer;

        private Vector3[][] _floorsPath;

        private void Start()
        {
            FindPath(_pointA.position, _pointB.position);
        }

        private void FindPath(Vector3 positionA, Vector3 positionB)
        {
            Graphway.FindPath(positionA, positionB, DistributePathInFloors);
        }

        private void DistributePathInFloors(GwWaypoint[] path)
        {
            if (path == null)
                return;
            
            _lineRenderer.positionCount = path.Length;
            _lineRenderer.SetPositions(path.
                Where(pathPoint => pathPoint.nodeID.HasValue).
                Select(pathPoint => pathPoint.position).ToArray());
        }
    }
}





