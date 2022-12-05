using System;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<MarkerPoint> _markerPoints;
    [SerializeField] private List<TargetPoint> _targetPoints;

    public List<MarkerPoint> MarkerPoints => new List<MarkerPoint>(_markerPoints);
    public List<TargetPoint> TargetPoints => new List<TargetPoint>(_targetPoints);


    public bool TryGetMarkerPoint(string name, out MarkerPoint foundPoint)
    {
        foundPoint = null;
        
        foreach (var point in _markerPoints)
        {
            if (point.Id == name)
            {
                foundPoint = point;
                return true;
            }
        }

        return false;
    }
    
    public bool TryGetTargetPoint(string name, out TargetPoint foundPoint)
    {
        foundPoint = null;
        
        foreach (var point in _targetPoints)
        {
            if (point.Id == name || point.Aliases.Contains(name))
            {
                foundPoint = point;
                return true;
            }
        }

        return false;
    }
}


[Serializable]
public abstract class Point
{
    [SerializeField] private string _id;

    public string Id => _id;
    public abstract Transform Transform { get; }
}

[Serializable]
public class MarkerPoint : Point
{
    [SerializeField] private VirtualMarker _virtualMarker;

    public VirtualMarker VirtualMarker => _virtualMarker;
    public override Transform Transform => _virtualMarker.transform;
}

[Serializable]
public class TargetPoint : Point
{
    [SerializeField] private List<string> _aliases;
    [SerializeField] private Transform _transform;
    
    public List<string> Aliases => new List<string>(_aliases);
    public override Transform Transform => _transform;
}
