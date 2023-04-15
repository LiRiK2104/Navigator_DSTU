using System.Collections.Generic;
using System.Linq;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using UnityEngine;

namespace Helpers.Tests
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private TestDataBase _testDataBase;

        private Vector3[][] _floorsPath;


        private void FindPath(Vector3 positionA, Vector3 positionB, int startFloor)
        {
            if (_testDataBase.TryGetFloorNodesIds(startFloor, out List<int> availableNodeIds))
                Graphway.FindPath(positionA, positionB, DistributePathInFloors, availableNodeIds.ToArray());
            else
                Graphway.FindPath(positionA, positionB, DistributePathInFloors);
        }

        private void DistributePathInFloors(GwWaypoint[] path)
        {
            if (path == null)
                return;

            _floorsPath = new Vector3[_testDataBase.FloorsCount][];
            
            for (int i = 0; i < _floorsPath.Length; i++)
            {
                if (_testDataBase.TryGetFloorNodesPositions(i, out List<Vector3> floorNodesIds))
                {
                    _floorsPath[i] = path.
                        Where(point => floorNodesIds.Contains(point.position)).
                        Select(point => point.position).
                        ToArray();
                }
            }
        }
    }
}





