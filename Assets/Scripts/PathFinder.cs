using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private Transform _user;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private NavMeshPath _path;

    private void Start()
    {
        _path = new NavMeshPath();
        StartCoroutine(PathFinding());
    }

    private void Update()
    {
        DrawPath();
    }

    private IEnumerator PathFinding()
    {
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
