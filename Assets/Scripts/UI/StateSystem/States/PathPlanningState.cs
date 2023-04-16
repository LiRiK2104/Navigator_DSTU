using System;
using Navigation;
using TargetsSystem.Points;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathPlanningState : State
    {
        [SerializeField] private PathPlanningView _pathPlanningView;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;


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
            
            var pathPoint = new PathPoint(point.GraphwayNodePosition, pointInfo.Address.FloorIndex);

            switch (fillingType)
            {
                case FillingPathFieldType.A:
                    SetA(pointInfo, pathPoint);
                    break;
             
                default:
                case FillingPathFieldType.B:
                    SetB(pointInfo, pathPoint);
                    break;
                
                case FillingPathFieldType.Priority:
                    SetPriority(pointInfo, pathPoint);
                    break;
            }
            
            _pathPlanningView.InitializePathInfoPanel();
        }

        public void Clear()
        {
            _pathPlanningView.Clear();
            PathFinder.ClearPath();
        }

        private void SetA(PointInfo pointInfo, PathPoint pathPoint)
        {
            _pathPlanningView.SetTextPointAField(pointInfo.Name);
            PathFinder.SetA(pathPoint);
        }
        
        private void SetB(PointInfo pointInfo, PathPoint pathPoint)
        {
            _pathPlanningView.SetTextPointBField(pointInfo.Name);
            PathFinder.SetB(pathPoint);
        }
        
        private void SetPriority(PointInfo pointInfo, PathPoint pathPoint)
        {
            switch (PathFinder.PriorityPoint)
            {
                case DestinationPoint.A:
                    SetA(pointInfo, pathPoint);
                    break;
                
                default:
                case DestinationPoint.B:
                    SetB(pointInfo, pathPoint);
                    break;
            }
        }

        private void Swap()
        {
            _pathPlanningView.SwapFields();
            PathFinder.Swap();
        }
    }

    public enum FillingPathFieldType
    {
        A, 
        B, 
        Priority
    }
}
