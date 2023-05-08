using System;
using Map;
using Navigation;
using TargetsSystem.Points;
using UI.FloorsSwitch;
using UI.Views;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathPlanningState : State
    {
        [SerializeField] private PathPlanningView _pathPlanningView;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;


        private void OnEnable()
        {
            PathFinder.PathFound += _pathPlanningView.InitializePathInfoPanel;
        }

        private void OnDisable()
        {
            PathFinder.PathFound -= _pathPlanningView.InitializePathInfoPanel;
        }
        

        public override void OnOpen()
        {
            _pathPlanningView.InitializePathInfoPanel();
        }

        public override void OnClose() { }

        public void InitializeView()
        {
            _pathPlanningView.Initialize(Swap);
        }
        
        public void SetPoint(PointInfo pointInfo, FillingPathFieldType fillingType)
        {
            if (DataBase.TryGetPoint(pointInfo, out Point point) == false)
                return;
            
            SetPoint(point.GraphwayNodePosition, pointInfo.Address.FloorIndex, pointInfo.Name, fillingType);
        }

        public void SetPoint(Vector3 position, int floorIndex, string name, FillingPathFieldType fillingType)
        {
            var pathPoint = new PathPoint(position, floorIndex);

            switch (fillingType)
            {
                case FillingPathFieldType.A:
                    SetA(name, pathPoint);
                    break;
             
                default:
                case FillingPathFieldType.B:
                    SetB(name, pathPoint);
                    break;
                
                case FillingPathFieldType.Priority:
                    SetPriority(name, pathPoint);
                    break;
            }
            
            _pathPlanningView.InitializePathInfoPanel();
        }

        public void Clear()
        {
            _pathPlanningView.Clear();
            PathFinder.ClearPath();
        }

        private void SetA(string pointName, PathPoint pathPoint)
        {
            _pathPlanningView.SetTextPointAField(pointName);
            PathFinder.SetA(pathPoint);
            UpdatePointer(pathPoint, FloorsSwitcher.CurrentFloorIndex, PointerState.PointA);
        }
        
        private void SetB(string pointName, PathPoint pathPoint)
        {
            _pathPlanningView.SetTextPointBField(pointName);
            PathFinder.SetB(pathPoint);
            UpdatePointer(pathPoint, FloorsSwitcher.CurrentFloorIndex, PointerState.PointB);
        }
        
        private void SetPriority(string pointName, PathPoint pathPoint)
        {
            switch (PathFinder.PriorityPoint)
            {
                case DestinationPoint.A:
                    SetA(pointName, pathPoint);
                    break;
                
                default:
                case DestinationPoint.B:
                    SetB(pointName, pathPoint);
                    break;
            }
        }

        private void Swap()
        {
            _pathPlanningView.SwapFields();
            PathFinder.Swap();
            UpdatePointers(FloorsSwitcher.CurrentFloorIndex);
        }

        private void UpdatePointers(int currentFloor)
        {
            UpdatePointer(PathFinder.PointA, currentFloor, PointerState.PointA);
            UpdatePointer(PathFinder.PointB, currentFloor, PointerState.PointB);
        }
        
        private void UpdatePointer(PathPoint? pathPoint, int currentFloorIndex, PointerState pointerState)
        {
            if (pathPoint != null)
            {
                SetPointer(pathPoint, pointerState, pathPoint.FloorIndex);
                
                if (currentFloorIndex != pathPoint.FloorIndex)
                    HidePointer(false, pointerState);
            }
            else
            {
                HidePointer(true, pointerState);
            }
        }
        
        private void SetPointer(PathPoint pathPoint, PointerState pointerState, int floorIndex)
        { 
            MapPointerSetter.SetPointer(new PointerSetRequest(pathPoint.Position, pointerState), floorIndex);
        }
        
        private void HidePointer(bool clearFloor, PointerState pointerState)
        { 
            MapPointerSetter.HidePointers(clearFloor, pointerState);
        }
    }

    public enum FillingPathFieldType
    {
        A, 
        B, 
        Priority
    }
}
