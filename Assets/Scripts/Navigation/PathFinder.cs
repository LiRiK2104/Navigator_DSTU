using System;
using System.Collections.Generic;
using System.Linq;
using Calibration;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using TargetsSystem.Points;
using UnityEngine;

namespace Navigation
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private List<LineRenderer> _lineRenderers;
        [SerializeField] private Graphway _graphway;

        private Vector3 _targetPosition;

        private PathPoint? _pointA;
        private PathPoint? _pointB;
        private Vector3[][] _floorsPath;
        
        private Vector3? _positionA;
        private Vector3? _positionB;

        public DestinationPoint PriorityPoint { get; set; }
        private Calibrator Calibrator => Global.Instance.Calibrator;
        private Transform UserTransform => Global.Instance.ArMain.CameraManager.transform;
        private DataBase DataBase => Global.Instance.DataBase;


        private void OnEnable()
        {
            /*Calibrator.Calibrated += ResetPath;
            Calibrator.Calibrated += ShowPath;
            Calibrator.CalibrationReset += HidePath;*/
        }

        private void OnDisable()
        {
            /*Calibrator.Calibrated -= ResetPath;
            Calibrator.Calibrated -= ShowPath;
            Calibrator.CalibrationReset -= HidePath;*/
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
        
        public Point GetNearestPoint(PathPoint pointA, PointsGroup pointsGroup)
        {
            return pointsGroup.Points.OrderBy(point =>
            {
                point.TryGetInfo(out PointInfo pointInfo);
                var pointB = new PathPoint(point.GraphwayNodePosition, pointInfo.Address.FloorIndex);
                var floorsPath = FindPath(pointA, pointB);
                return CalculatePathDistance(floorsPath);
            }).First();
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
            _floorsPath = null;
            var floorIndex = 0;
            DrawPath(floorIndex);
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

        private void ResetPath()
        {
            /*if (_targetPosition == default)
                return;
            
            ClearPath();
            FindPath();
            DrawPath();*/
        }

        private void FindPath()
        {
            if (_pointA.HasValue == false || 
                _pointB.HasValue == false) 
                return;

            _floorsPath = FindPath(_pointA.Value, _pointB.Value);
        }
        
        private Vector3[][] FindPath(PathPoint pointA, PathPoint pointB)
        {
            var startFloorIndex = pointA.FloorIndex;

            Vector3[][] floorsPath = null;
            Action<GwWaypoint[]> callback = path => { DistributePathInFloors(path, out floorsPath); };

            if (DataBase.TryGetFloorNodesIds(startFloorIndex, out List<int> availableNodeIds))
                _graphway.PathFind(pointA.Position, pointB.Position, callback, availableNodeIds.ToArray());
            else
                _graphway.PathFind(pointA.Position, pointB.Position, callback);

            return floorsPath;
        }

        private void DistributePathInFloors(GwWaypoint[] path, out Vector3[][] floorsPath)
        {
            floorsPath = new Vector3[DataBase.Floors.Count][];
            
            if (path == null)
                return;

            for (int i = 0; i < floorsPath.Length; i++)
            {
                if (DataBase.TryGetFloorNodesPositions(i, out List<Vector3> floorNodesIds))
                {
                    floorsPath[i] = path.
                        Where(point => floorNodesIds.Contains(point.position)).
                        Select(point => point.position).
                        ToArray();
                }
            }
        }

        private void DrawPath(int floorIndex)
        {
            if (_floorsPath == null || _floorsPath.Length == 0)
                return;
            
            //TODO: OnFindPath, OnSwitchFloor
            var floorPath = _floorsPath[floorIndex];
            
            foreach (var lineRenderer in _lineRenderers)
            {
                for (int i = 0; i < floorPath.Length - 1; i++)
                {
                    lineRenderer.positionCount = floorPath.Length;
                    lineRenderer.SetPositions(floorPath);
                }   
            }
        }

        private void ShowPath()
        {
            foreach (var lineRenderer in _lineRenderers)
                lineRenderer.enabled = true;
        }
        
        private void HidePath()
        {
            foreach (var lineRenderer in _lineRenderers)
                lineRenderer.enabled = false;
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
