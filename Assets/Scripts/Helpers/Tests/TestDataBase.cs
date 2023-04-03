using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers.Tests
{
    public class TestDataBase : MonoBehaviour
    {
        [SerializeField] private List<List<GraphwayNode>> _floorsNodes = new List<List<GraphwayNode>>();

        public int FloorsCount => _floorsNodes.Count;
        
        public bool TryGetFloorNodesIds(int floorIndex, out List<int> ids)
        {
            ids = new List<int>();
            
            if (floorIndex < 0 || floorIndex >= _floorsNodes.Count)
                return false;

            ids = _floorsNodes[floorIndex].Select(node => node.nodeID).ToList();

            return ids.Count != 0;
        }
        
        public bool TryGetFloorNodesPositions(int floorIndex, out List<Vector3> positions)
        {
            positions = new List<Vector3>();
            
            if (floorIndex < 0 || floorIndex >= _floorsNodes.Count)
                return false;

            positions = _floorsNodes[floorIndex].Select(node => node.transform.position).ToList();

            return positions.Count != 0;
        }
    }
}
