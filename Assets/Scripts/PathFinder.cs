using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private Transform _user;
    [SerializeField] private Transform _targetPoint;
    
    private NavMeshPath _path;

    private void Start()
    {
        _path = new NavMeshPath();
        StartCoroutine(PathFinding());
    }

    private void Update()
    {
        for (int i = 0; i < _path.corners.Length - 1; i++)
            Debug.DrawLine(_path.corners[i], _path.corners[i+1], Color.red);
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
}
