using System;
using System.Collections.Generic;
using TargetsSystem.Rooms;
using UnityEngine;

namespace Navigation
{
    public class DestinationSetter : MonoBehaviour
    {
        public event Action<Vector3> TargetSet;

        public bool HasDestination { get; private set; }
        private DataBase DataBase => Global.Instance.DataBase;
        private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
        private SearchableDropDown TargetsDropdown => Global.Instance.UiSetter.TrackingMenu.TargetsDropdown;
    

        private void OnEnable()
        {
            TargetsDropdown.ValueChanged += SetTarget;
        }

        private void OnDisable()
        {
            TargetsDropdown.ValueChanged -= SetTarget;
        }

        private void Start()
        {
            InitializeSearch();
        }

    
        private void InitializeSearch()
        {
            var targetsNames = new List<string>();
        
            DataBase.Rooms.ForEach(room => targetsNames.Add(room.Id));
            
            foreach (var multiRoom in DataBase.MultiRooms)
                multiRoom.GetAllIds().ForEach(id => targetsNames.Add(id));
        
            targetsNames.Sort();
            TargetsDropdown.Initialize(targetsNames);
        }

        private void SetTarget(string targetName)
        {
            if (DataBase.TryGetRoom(targetName, out AccessibleRoom room))
            {
                PathFinder.SetTarget(room.TargetPointPosition);
                HasDestination = true;
                TargetSet?.Invoke(room.TargetPointPosition);
            }
        }
    }
}
