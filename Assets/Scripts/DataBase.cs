using System;
using System.Collections.Generic;
using System.Linq;
using Calibration;
using TargetsSystem.Rooms;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<TriadMarker> _triadMarkers;
    [SerializeField] private List<AccessibleRoom> _rooms;
    [SerializeField] private List<MultiRoom> _multiRooms;

    public List<TriadMarker> TriadMarker => new List<TriadMarker>(_triadMarkers);
    public List<AccessibleRoom> Rooms => new List<AccessibleRoom>(_rooms);
    public List<MultiRoom> MultiRooms => new List<MultiRoom>(_multiRooms);


    public bool TryGetVirtualMarker(List<ARTrackedImage> trackedImages, 
        out TriadMarker foundMarker,
        out ARTrackedImage image1st, 
        out ARTrackedImage image2nd, 
        out ARTrackedImage image3rd)
    {
        foundMarker = null;
        image1st = null;
        image2nd = null;
        image3rd = null;

        foreach (var triadMarker in _triadMarkers)
        {
            if (triadMarker.Triad.HasTrackedImage(trackedImages, out image1st, out image2nd, out image3rd))
            {
                foundMarker = triadMarker;
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
