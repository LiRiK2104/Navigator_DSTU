using System;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
    public class DestinationSetter : MonoBehaviour
    {
        public event Action<TargetPoint> TargetSet;

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
        
            foreach (var point in DataBase.TargetPoints)
            {
                targetsNames.Add(point.Id);
                point.Aliases.ForEach(alias => targetsNames.Add(alias));
            }
        
            targetsNames.Sort();
            TargetsDropdown.Initialize(targetsNames);
        }

        private void SetTarget(string targetName)
        {
            if (DataBase.TryGetTargetPoint(targetName, out TargetPoint foundPoint))
            {
                PathFinder.SetTarget(foundPoint.Transform);
                HasDestination = true;
                TargetSet?.Invoke(foundPoint);
            }
        }
    }
}
