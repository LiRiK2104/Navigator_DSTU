using System;
using UnityEngine;

namespace Navigation
{
    public class DestinationSetter : MonoBehaviour
    {
        public event Action<Vector3> TargetSet;

        public bool HasDestination { get; private set; }
        private DataBase DataBase => Global.Instance.DataBase;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;


        private void Start()
        {
            InitializeSearch();
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
