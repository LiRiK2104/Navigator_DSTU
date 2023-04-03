using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
    public class PathDrawer : MonoBehaviour
    {
        [SerializeField] private List<LineRenderer> _lineRenderers;
        
        //TODO: OnFindPath, OnSwitchFloor
        
        
        private void DrawPath(Vector3[] floorPath)
        {
            foreach (var lineRenderer in _lineRenderers)
            {
                lineRenderer.positionCount = floorPath.Length;
                lineRenderer.SetPositions(floorPath);
            }
        }
    }
}
