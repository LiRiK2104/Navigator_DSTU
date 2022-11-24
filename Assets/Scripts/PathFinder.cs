using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private Transform _user;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private Transform _targetPoint;
    private NavMeshPath _path;

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

    
    private void Start()
    {
        _path = new NavMeshPath();
    }

    private void Update()
    {
        DrawPath();
    }
    

    public void SetTarget(Transform targetPoint)
    {
        _targetPoint = targetPoint;
        StartCoroutine(FindPath());
    }

    private IEnumerator FindPath()
    {
        if (_targetPoint == null)
            yield break;
        
        float updatingTime = 0.2f;
        
        while (true)
        {
            NavMesh.CalculatePath(_user.position, _targetPoint.position, NavMesh.AllAreas, _path);
            yield return new WaitForSeconds(updatingTime);
        }
    }

    private void DrawPath()
    {
        for (int i = 0; i < _path.corners.Length - 1; i++)
        {
            _lineRenderer.positionCount = _path.corners.Length;
            _lineRenderer.SetPositions(_path.corners);
        }
    }
}
