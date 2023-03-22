using System.Collections.Generic;
using Calibration;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private List<LineRenderer> _lineRenderers;

        private Vector3 _targetPosition;
        private NavMeshPath _path;

        private Calibrator Calibrator => Global.Instance.Calibrator;
        private Transform UserTransform => Global.Instance.ArMain.CameraManager.transform;

        public Vector3 NearestPoint
        {
            get
            {
                _path ??= new NavMeshPath();
                int minLength = 2;

                if (_path.corners.Length >= minLength)
                    return _path.corners[minLength - 1];

                return Vector3.zero;
            }
        }


        private void OnEnable()
        {
            Calibrator.Calibrated += ResetPath;
            Calibrator.Calibrated += ShowPath;
            Calibrator.CalibrationReset += HidePath;
        }

        private void OnDisable()
        {
            Calibrator.Calibrated -= ResetPath;
            Calibrator.Calibrated -= ShowPath;
            Calibrator.CalibrationReset -= HidePath;
        }

        private void Start()
        {
            _path = new NavMeshPath();
        }


        public NavMeshPath GetLocalPath(Vector3 targetPosition)
        {
            var localPath = new NavMeshPath();
            NavMesh.CalculatePath(UserTransform.position, targetPosition, NavMesh.AllAreas, localPath);

            return localPath;
        }

        public float CalculatePathDistance(NavMeshPath path)
        {
            float distance = 0;

            for (int i = 0; i < path.corners.Length - 1; i++)
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);

            return distance;
        }
        
        public void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            ResetPath();
        }

        private void ResetPath()
        {
            if (_targetPosition == default)
                return;
            
            ClearPath();
            FindPath();
            DrawPath();
        }

        private void ClearPath()
        {
            _path.ClearCorners();
        }

        private void FindPath()
        {
            NavMesh.CalculatePath(UserTransform.position, _targetPosition, NavMesh.AllAreas, _path);
        }

        private void DrawPath()
        {
            foreach (var lineRenderer in _lineRenderers)
            {
                for (int i = 0; i < _path.corners.Length - 1; i++)
                {
                    lineRenderer.positionCount = _path.corners.Length;
                    lineRenderer.SetPositions(_path.corners);
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
}
