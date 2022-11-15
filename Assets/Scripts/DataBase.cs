using System;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<Point> _points = new List<Point>();

    public bool TryGetPoint(string name, out Point foundPoint)
    {
        foundPoint = null;
        
        foreach (var point in _points)
        {
            if (point.Name == name)
            {
                foundPoint = point;
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class Point
{
    [SerializeField] private string _name;
    [SerializeField] private VirtualMarker _virtualMarker;

    public string Name => _name;
    public VirtualMarker VirtualMarker => _virtualMarker;
}
