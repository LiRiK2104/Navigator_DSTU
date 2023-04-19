using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calibration;
using Map;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using TargetsSystem.Points;
using UI.FloorsSwitch;
using UnityEngine;

namespace Navigation
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private LineRenderer _mapLineRenderer;
        [SerializeField] private Graphway _graphway;

        private Vector3 _targetPosition;

        private PathPoint? _pointA;
        private PathPoint? _pointB;
        private Vector3[][] _floorsPath;

        private delegate void FindPathHandler(Vector3[][] pathFloors);
        public event Action PathFound;

        public PathPoint? PointA => _pointA;
        public PathPoint? PointB => _pointB;
        public DestinationPoint PriorityPoint { get; set; }
        
        private Calibrator Calibrator => Global.Instance.Calibrator;
        private Transform UserTransform => Global.Instance.ArMain.CameraManager.transform;
        private DataBase DataBase => Global.Instance.DataBase;
        private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;


        private void OnEnable()
        {
            /*Calibrator.Calibrated += ResetPath;
            Calibrator.Calibrated += ShowPath;
            Calibrator.CalibrationReset += HidePath;*/
            PathFound += DrawFloorPath;
            FloorsSwitcher.FloorSwitched += DrawFloorPath;
        }

        private void OnDisable()
        {
            /*Calibrator.Calibrated -= ResetPath;
            Calibrator.Calibrated -= ShowPath;
            Calibrator.CalibrationReset -= HidePath;*/
            PathFound -= DrawFloorPath;
            FloorsSwitcher.FloorSwitched -= DrawFloorPath;
        }


        public void SetA(PathPoint pathPoint)
        {
            _pointA = pathPoint;
            FindPath();
        }
        
        public void SetB(PathPoint pathPoint)
        {
            _pointB = pathPoint;
            FindPath();
        }
        
        public void Swap()
        {
            PathPoint? pointA = _pointA;
            PathPoint? pointB = _pointB;
            _pointA = pointB;
            _pointB = pointA;
            
            FindPath();
        }

        public Vector3 GetNearestPathPoint(PathPoint startPoint)
        {
            return _floorsPath[startPoint.FloorIndex].OrderBy(floorPoint => Vector3.Distance(startPoint.Position, floorPoint))
                .FirstOrDefault();
        }
        
        public IEnumerator FindNearestPoint(PathPoint pointA, PointsGroup pointsGroup, Action<Point> callback)
        {
            Point nearestPoint = null;
            float? minDistance = null;
            
            foreach (var groupPoint in pointsGroup.Points)
            {
                groupPoint.TryGetInfo(out PointInfo pointInfo);
                var pointB = new PathPoint(groupPoint.GraphwayNodePosition, pointInfo.Address.FloorIndex);
                FindPathHandler findPathCallback = floorsPath =>
                {
                    var distance = CalculatePathDistance(floorsPath);

                    if (distance < minDistance || minDistance.HasValue == false)
                    {
                        minDistance = distance;
                        nearestPoint = groupPoint;
                    }
                };

                yield return FindPath(pointA, pointB, findPathCallback);
            }

            if (nearestPoint != null)
                callback.Invoke(nearestPoint);
        }

        public bool TryGetCurrentPathDistance(out float distance)
        {
            distance = 0;
            
            if (_floorsPath == null)
                return false;

            distance = CalculatePathDistance(_floorsPath);
            return true;
        }
        
        public void ClearPath()
        {
            _pointA = null;
            _pointB = null;
            _floorsPath = null;
            DrawEmptyPath();
        }
        
        private float CalculatePathDistance(Vector3[][] floorsPath)
        {
            float distance = 0;

            for (int i = 0; i < floorsPath.Length; i++)
            {
                Vector3[] floorPath = _floorsPath[i];
                
                for (int j = 0; j < floorPath.Length - 1; j++)
                    distance += Vector3.Distance(floorPath[j], floorPath[j + 1]);
            }

            return distance;
        }

        private void FindPath()
        {
            if (_pointA.HasValue == false || 
                _pointB.HasValue == false) 
                return;
            
            StartCoroutine(FindPath(_pointA.Value, _pointB.Value, SetFloorsPath));
        }
        
        private IEnumerator FindPath(PathPoint pointA, PathPoint pointB, params FindPathHandler[] callbacks)
        {
            int startFloorIndex = pointA.FloorIndex;

            Action<GwWaypoint[]> graphwayCallback = path =>
            {
                DistributePathInFloors(pointA.Position, path, out Vector3[][] floorsPath);

                foreach (var callback in callbacks)
                    callback.Invoke(floorsPath);
            };

            if (DataBase.TryGetFloorNodesIds(startFloorIndex, out List<int> availableNodeIds))
                _graphway.PathFind(pointA.Position, pointB.Position, graphwayCallback, availableNodeIds.ToArray());
            else
                _graphway.PathFind(pointA.Position, pointB.Position, graphwayCallback);

            yield break;
        }
        
        private void SetFloorsPath(Vector3[][] floorsPath)
        {
            _floorsPath = floorsPath;
            PathFound?.Invoke();
        }

        private void DistributePathInFloors(Vector3 startPosition, GwWaypoint[] path, out Vector3[][] floorsPath)
        {
            floorsPath = new Vector3[DataBase.Floors.Count][];
            
            if (path == null)
                return;

            for (int i = 0; i < floorsPath.Length; i++)
            {
                var floorNodesIDs = ArEnvironment.FirstBuilding.Floors[i].Graphway.GetAllNodes()
                    .Select(node => node.nodeID).ToList();

                var firstPathPoint = path[0];
                var pathFloorNodesPositions = new List<Vector3>();
                
                if (firstPathPoint.nodeID.HasValue && 
                    floorNodesIDs.Contains(firstPathPoint.nodeID.Value) && 
                    firstPathPoint.position != startPosition)
                {
                    pathFloorNodesPositions.Add(startPosition);
                }

                for (int j = 0; j < path.Length; j++)
                {
                    if (path[j].nodeID.HasValue && floorNodesIDs.Contains(path[j].nodeID.Value))
                        pathFloorNodesPositions.Add(path[j].position);
                }

                floorsPath[i] = pathFloorNodesPositions.ToArray();
            }
        }

        private void DrawFloorPath()
        {
            DrawFloorPath(FloorsSwitcher.CurrentFloorIndex);
        }

        private void DrawFloorPath(int floorIndex)
        {
            if (_floorsPath == null || _floorsPath.Length == 0)
                return;
            
            var floorPath = _floorsPath[floorIndex];
            var minPointsCount = 2;

            if (floorPath.Length < minPointsCount)
                DrawEmptyPath();
            else
                DrawPath(floorPath, _mapLineRenderer);
        }

        private void DrawPath(Vector3[] positions, params LineRenderer[] lineRenderers)
        {
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.positionCount = positions.Length;
                lineRenderer.SetPositions(positions);
            }
        }

        private void DrawEmptyPath()
        {
            DrawPath(Array.Empty<Vector3>(), _mapLineRenderer);
        }
    }

    public struct PathPoint
    {
        public Vector3 Position { get; }
        public int FloorIndex { get; }

        public PathPoint(Vector3 position, int floorIndex)
        {
            Position = position;
            FloorIndex = floorIndex;
        }
    }
    
    public enum DestinationPoint
    {
        A,
        B
    }
}
