using System;
using System.Collections.Generic;
using TargetsSystem.Rooms;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<VirtualMarker> _virtualMarkers;
    [SerializeField] private List<AccessibleRoom> _rooms;
    [SerializeField] private List<MultiRoom> _multiRooms;

    public List<VirtualMarker> VirtualMarkers => new List<VirtualMarker>(_virtualMarkers);
    public List<AccessibleRoom> Rooms => new List<AccessibleRoom>(_rooms);
    public List<MultiRoom> MultiRooms => new List<MultiRoom>(_multiRooms);


    public bool TryGetVirtualMarker(string name, out VirtualMarker foundPoint)
    {
        foundPoint = null;
        
        foreach (var point in _virtualMarkers)
        {
            if (point.Id == name)
            {
                foundPoint = point;
                return true;
            }
        }

        return false;
    }
    
    public bool TryGetRoom(string name, out AccessibleRoom foundRoom)
    {
        foundRoom = null;
        
        foreach (var room in _rooms)
        {
            if (room.Id == name)
            {
                foundRoom = room;
                return true;
            }
        }
        
        foreach (var multiRoom in _multiRooms)
        {
            if (multiRoom.CommonId == name)
            {
                foundRoom = multiRoom.GetNearestRoom();
                return true;
            }
                
            if (multiRoom.TryGetRoom(name, out foundRoom))
                return true;
        }

        return false;
    }
}


/*[Serializable]
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
public class TargetPointOld : Point
{
    [SerializeField] private List<string> _aliases;
    [SerializeField] private Transform _transform;
    
    public List<string> Aliases => new List<string>(_aliases);
    public override Transform Transform => _transform;
}*/
