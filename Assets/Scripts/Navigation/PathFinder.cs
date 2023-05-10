using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AR.Calibration;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using TargetsSystem.Points;
using UI.FloorsSwitch;
using UnityEngine;

namespace Navigation
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private LineRenderer _mapLineRenderer;
        [SerializeField] private LineRenderer _worldviewLineRenderer;
        [SerializeField] private Graphway _graphway;

        private Vector3 _targetPosition;

        private Vector3[][] _floorsPath;

        private delegate void FindPathHandler(Vector3[][] pathFloors);
        public event Action PathFound;
        public event Action<int> PointASet;
        public event Action<int> PointBSet;
        public event Action PointARemoved;
        public event Action PointBRemoved;

        public PathPoint PointA { get; private set; }
        public PathPoint PointB { get; private set; }

        public DestinationPoint PriorityPoint { get; set; }
        
        private Calibrator Calibrator => Global.Instance.ArMain.Calibrator;
        private DataBase DataBase => Global.Instance.DataBase;
        private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;

        
        private void Awake()
        {
            _mapLineRenderer.gameObject.SetActive(false);
            _worldviewLineRenderer.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Calibrator.Completed += UpdatePath;
            PathFound += DrawFloorPath;
            FloorsSwitcher.FloorSwitched += DrawFloorPath;
        }

        private void OnDisable()
        {
            Calibrator.Completed -= UpdatePath;
            PathFound -= DrawFloorPath;
            FloorsSwitcher.FloorSwitched -= DrawFloorPath;
        }


        public void SetA(PathPoint pathPoint)
        {
            PointA = pathPoint;
            PointASet?.Invoke(pathPoint.FloorIndex);
            FindPath();
        }
        
        public void SetB(PathPoint pathPoint)
        {
            PointB = pathPoint;
            PointBSet?.Invoke(pathPoint.FloorIndex);
            FindPath();
        }
        
        public void Swap()
        {
            var pointA = PointA;
            var pointB = PointB;
            PointA = pointB;
            PointB = pointA;

            if (PointA != null)
                PointASet?.Invoke(PointA.FloorIndex);
            else 
                PointARemoved?.Invoke();
            
            if (PointB != null)
                PointBSet?.Invoke(PointB.FloorIndex);
            else 
                PointBRemoved?.Invoke();
            
            FindPath();
        }

        public Vector3 GetGuidingPathPoint(PathPoint startPoint)
        {
            const int maxPointOrder = 7;
            var floorPathPointsDistances = _floorsPath[startPoint.FloorIndex]
                .OrderBy(floorPoint => Vector3.Distance(startPoint.Position, floorPoint)).ToArray();

            var pointOrder = Mathf.Min(floorPathPointsDistances.Length, maxPointOrder);
            return floorPathPointsDistances[pointOrder - 1];
        }
        
        public IEnumerator FindNearestPoint(PathPoint startPoint, PointsGroup pointsGroup, Action<Point> callback)
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

                yield return FindPath(startPoint, pointB, findPathCallback);
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
            PointA = null;
            PointB = null;
            PointARemoved?.Invoke();
            PointBRemoved?.Invoke();
            _floorsPath = null;
            DrawEmptyPath();
        }
        
        private float CalculatePathDistance(Vector3[][] floorsPath)
        {
            float distance = 0;

            for (int i = 0; i < floorsPath.Length; i++)
            {
                Vector3[] floorPath = floorsPath[i];
                
                for (int j = 0; j < floorPath.Length - 1; j++)
                    distance += Vector3.Distance(floorPath[j], floorPath[j + 1]);
            }

            return distance;
        }

        private void UpdatePathPoints()
        {
            PointA?.UpdatePosition();
            PointB?.UpdatePosition();
        }

        private void UpdatePath()
        {
            UpdatePathPoints();
            FindPath();
        }

        private void FindPath()
        {
            if (PointA == null || PointB == null) 
                return;
            
            StartCoroutine(FindPath(PointA, PointB, SetFloorsPath));
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
                _graphway.PathFind(pointA.Position, pointB.Position, graphwayCallback, availableNodeIds.ToArray(), false);
            else
                _graphway.PathFind(pointA.Position, pointB.Position, graphwayCallback, false);

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
            
            const int minPointsCount = 2;
            var floorPath = _floorsPath[floorIndex];

            if (floorPath.Length < minPointsCount)
                DrawEmptyPath();
            else
                DrawPath(floorPath, _mapLineRenderer, _worldviewLineRenderer);
        }

        private void DrawPath(Vector3[] positions, params LineRenderer[] lineRenderers)
        {
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.gameObject.SetActive(positions.Length > 0);
                lineRenderer.positionCount = positions.Length;
                lineRenderer.SetPositions(positions);
            }
        }

        private void DrawEmptyPath()
        {
            DrawPath(Array.Empty<Vector3>(), _mapLineRenderer, _worldviewLineRenderer);
        }
    }

    public class PathPoint
    {
        private readonly Vector3 _relativeEnvironmentPosition;
        private readonly Quaternion _environmentStartRotation;
        
        public Vector3 Position { get; private set; }
        public int FloorIndex { get; }
        private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;

        public PathPoint(Vector3 position, int floorIndex)
        {
            Position = position;
            FloorIndex = floorIndex;

            _relativeEnvironmentPosition = Position - ArEnvironment.transform.position;
            _environmentStartRotation = ArEnvironment.transform.rotation;
        }

        public void UpdatePosition()
        {
            Position = ArEnvironment.transform.position + 
                       (Quaternion.Inverse(_environmentStartRotation) * ArEnvironment.transform.rotation) * 
                       _relativeEnvironmentPosition;
        }
    }
    
    public enum DestinationPoint
    {
        A,
        B
    }
}
