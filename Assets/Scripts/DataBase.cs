using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AR.Calibration;
using Helpers;
using Map.Signs;
using Navigation;
using TargetsSystem.Points;
using UI.Search.Options;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DataBase : MonoBehaviour
{
    [SerializeField] private List<PointsGroup> _pointsGroups;
    [SerializeField] private List<Floor> _floors;

    private Dictionary<Point, PointInfo> _pointInfos = new Dictionary<Point, PointInfo>();
    private List<TriadMarker> _allTriadMarkers;

    public List<PointsGroup> PointsGroups => new List<PointsGroup>(_pointsGroups);
    public List<Floor> Floors =>  new List<Floor>(_floors);
    private ReadOnlyCollection<TriadMarker> AllTriadMarkers
    {
        get
        {
            if (_allTriadMarkers.IsNullOrEmpty())
            {
                _allTriadMarkers = new List<TriadMarker>();
                
                foreach (var floor in _floors)
                {
                    foreach (var block in floor.Blocks)
                    {
                        foreach (var point in block.Points)
                        {
                            if (point is TriadMarker triadMarker)
                                _allTriadMarkers.Add(triadMarker);
                        }
                    }
                }
            }

            return _allTriadMarkers.AsReadOnly();
        }
    }
    
    
    public void Initialize()
    {
        FillPointInfos();
        
        foreach (var point in GetAllPoints())
            point.Initialize();

        foreach (var triadMarker in AllTriadMarkers)
            triadMarker.Initialize();
    }
    
    public bool TryGetVirtualMarker(List<ARTrackedImage> trackedImages, 
        out TriadMarker foundMarker,
        out ARTrackedImage image1st, 
        out ARTrackedImage image2nd, 
        out ARTrackedImage image3rd,
        out int floorIndex)
    {
        foundMarker = null;
        image1st = null;
        image2nd = null;
        image3rd = null;
        floorIndex = 0;

        for (int i = 0; i < _floors.Count; i++)
        {
            foreach (var block in _floors[i].Blocks)
            {
                foreach (var point in block.Points)
                {
                    if (point is TriadMarker triadMarker &&
                        triadMarker.Triad.HasTrackedImage(trackedImages, out image1st, out image2nd, out image3rd))
                    {
                        foundMarker = triadMarker;
                        floorIndex = i;
                        return true;
                    }
                }
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
    
    public bool TryGetFloorNodesIds(int floorIndex, out List<int> ids)
    {
        ids = null;
        
        if (floorIndex < 0 || floorIndex >= _floors.Count)
            return false;

        ids = (
            from block in _floors[floorIndex].Blocks 
            from point in block.Points
            where point.IsWayPoint
            select point.GraphwayNode.nodeID).ToList();

        return ids.Count != 0;
    }
        
    public bool TryGetFloorNodesPositions(int floorIndex, out List<Vector3> positions)
    {
        positions = null;
            
        if (floorIndex < 0 || floorIndex >= _floors.Count)
            return false;

        positions = (
            from block in _floors[floorIndex].Blocks 
            from point in block.Points 
            select point.GraphwayNodePosition).ToList();

        return positions.Count != 0;
    }

    public List<IOptionInfo> GetAllOptionInfos(bool onlyWayPoints = false)
    {
        var pointsOptionInfos = GetPointsOptionInfos(onlyWayPoints);
        var groupsOptionInfos = GetGroupsOptionInfos();
        
        return pointsOptionInfos.Concat(groupsOptionInfos).ToList();
    }

    public List<IOptionInfo> GetPointsOptionInfos(bool onlyWayPoints = false)
    {
        var points = GetAllPoints().ToList();

        if (onlyWayPoints)
            points = points.Where(point => point.IsWayPoint).ToList();
        
        var optionInfos = points.Select(point =>
        {
            TryGetPointInfo(point, out PointInfo pointInfo);
            return pointInfo as IOptionInfo;
        }).ToList();

        return optionInfos;
    }
    
    public List<IOptionInfo> GetGroupsOptionInfos()
    {
        return PointsGroups.Select(group => group as IOptionInfo).ToList();
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
        string id = point is AccessibleRoom accessibleRoom ? accessibleRoom.Id : String.Empty;
        var pointType = point.SignCreator.SignPreset.PointType;

        Address address = TryGetPointTypeNumber(point, out int pointTypeNumber) ? 
            new Address(floorIndex, blockName, id, pointType, pointTypeNumber) : 
            new Address(floorIndex, blockName, id);
        
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
            number = 0;

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


