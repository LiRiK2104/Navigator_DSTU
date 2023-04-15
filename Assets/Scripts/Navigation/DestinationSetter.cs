using System;
using TargetsSystem.Points;
using UnityEngine;

namespace Navigation
{
    public class DestinationSetter : MonoBehaviour
    {
        private Vector3 _positionA;
        private Vector3 _positionB;
        //private DestinationPoint _priorityPoint;
        
        public event Action<Vector3> TargetSet;

        public bool HasDestination { get; private set; }


        private void Start()
        {
            InitializeSearch();
        }

        public void SetA(Vector3 pointPosition)
        {
            _positionA = pointPosition;
        }
        
        public void SetB(Vector3 pointPosition)
        {
            _positionB = pointPosition;
        }
        
        /*public void SetToPriority(Vector3 pointPosition)
        {
            switch (_priorityPoint)
            {
                case DestinationPoint.A:
                    SetA(pointPosition);
                    break;
                
                default:
                case DestinationPoint.B:
                    SetB(pointPosition);
                    break;
            }
        }*/

        public void ClearPath()
        {
            
        }
    
        private void InitializeSearch()
        {
            /*var targetsNames = new List<string>();
        
            DataBase.Rooms.ForEach(room => targetsNames.Add(room.Id));
            
            foreach (var multiRoom in DataBase.PointsGroups)
                multiRoom.GetAllIds().ForEach(id => targetsNames.Add(id));
        
            targetsNames.Sort();
            TargetsDropdown.Initialize(targetsNames);*/
            //TODO: Сделать новыую реализацию
        }

        private void SetTarget(string targetName)
        {
            /*if (DataBase.TryGetRoom(targetName, out AccessibleRoom room))
            {
                PathFinder.SetTarget(room.TargetPointPosition);
                HasDestination = true;
                TargetSet?.Invoke(room.TargetPointPosition);
            }*/
            //TODO: Сделать новыую реализацию
        }
    }
}
