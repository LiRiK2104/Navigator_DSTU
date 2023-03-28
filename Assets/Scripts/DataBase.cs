using System;
using System.Collections.Generic;
using System.Linq;
using Calibration;
using Map.Signs;
using Navigation;
using TargetsSystem;
using TargetsSystem.Points;
using TargetsSystem.Rooms;
using UI.Search.Options;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<TriadMarker> _triadMarkers;
    [SerializeField] private List<PointsGroup> _pointsGroups;
    [SerializeField] private List<Floor> _floors;

    private Dictionary<Point, PointInfo> _pointInfos = new Dictionary<Point, PointInfo>();

    public List<TriadMarker> TriadMarker => new List<TriadMarker>(_triadMarkers);
    public List<PointsGroup> PointsGroups => new List<PointsGroup>(_pointsGroups);
    public List<Floor> Floors =>  new List<Floor>(_floors);


    private void Awake()
    {
        FillPointInfos();
    }
    
    
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
    

    public List<Point> GetAllPoints()
    {
        var allPoints = new List<Point>();
            
        foreach (var floor in _floors)
        {
            foreach (var block in floor.Blocks)
            {
                foreach (var point in block.Points)
                {
                    allPoints.Add(point);
                }
            }
        }

        return allPoints;
    }
    
    public bool TryGetPoint(PointInfo pointInfo, out Point point)
    {
        point = null;
        
        foreach (var localPointInfo in _pointInfos)
        {
            if (localPointInfo.Value.Equals(pointInfo))
            {
                point = localPointInfo.Key;
                return true;
            }
        }

        return false;
    }

    public bool TryGetPointInfo(Point targetPoint, out PointInfo pointInfo)
    {
        return _pointInfos.TryGetValue(targetPoint, out pointInfo);
    }

    private void FillPointInfos()
    {
        for (int i = 0; i < Floors.Count; i++)
        {
            foreach (var block in Floors[i].Blocks)
            {
                foreach (var point in block.Points)
                {
                    _pointInfos.Add(point, CreatePointInfo(point, i, block.Name));
                }
            }
        }
    }
    
    private PointInfo CreatePointInfo(Point point, int floorIndex, string blockName)
    {
        int floorNumber = floorIndex + 1;
        string id = point is AccessibleRoom accessibleRoom ? accessibleRoom.Id : "0";
        var pointType = point.SignCreator.SignPreset.PointType;

        Address address = TryGetPointTypeNumber(point, out int typerNumber) ? 
            new Address(floorNumber, blockName, id, pointType, typerNumber) : 
            new Address(floorNumber, blockName, id);
        
        return new PointInfo(point, address);
    }

    private bool TryGetPointTypeNumber(Point myPoint, out int number)
    {
        number = 0;
        var myPointType = myPoint.SignCreator.SignPreset.PointType;

        if (myPointType == PointType.None)
            return false;

        foreach (var floor in _floors)
        {
            foreach (var block in floor.Blocks)
            {
                foreach (var point in block.Points)
                {
                    var pointType = point.SignCreator.SignPreset.PointType;

                    if (pointType == myPointType)
                    {
                        number++;
                        
                        if (point == myPoint)
                            return true;
                    }
                }
            }
        }

        return false;
    }
    

    [Serializable]
    public class Floor
    {
        public List<Block> Blocks = new List<Block>();
    }
    
    [Serializable]
    public class Block
    {
        public string Name; 
        public List<Point> Points = new List<Point>();
    }
}


