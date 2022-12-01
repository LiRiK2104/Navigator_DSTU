using System;
using System.Collections.Generic;
using UnityEngine;

public class DestinationSetter : MonoBehaviour
{
    [SerializeField] private DataBase _dataBase;
    [SerializeField] private PathFinder _pathFinder;
    [SerializeField] private SearchableDropDown _searchableDropDown;
    [SerializeField] private PointersSetter _pointersSetter;

    public event Action TargetSet;

    public bool HasDestination { get; private set; }
    

    private void OnEnable()
    {
        _searchableDropDown.ValueChanged += SetTarget;
    }

    private void OnDisable()
    {
        _searchableDropDown.ValueChanged -= SetTarget;
    }

    private void Start()
    {
        InitializeSearch();
    }

    
    private void InitializeSearch()
    {
        var targetsNames = new List<string>();
        
        foreach (var point in _dataBase.TargetPoints)
        {
            targetsNames.Add(point.Id);
            point.Aliases.ForEach(alias => targetsNames.Add(alias));
        }
        
        targetsNames.Sort();
        _searchableDropDown.Initialize(targetsNames);
    }

    private void SetTarget(string targetName)
    {
        if (_dataBase.TryGetTargetPoint(targetName, out TargetPoint foundPoint))
        {
            _pathFinder.SetTarget(foundPoint.Transform);
            _pointersSetter.SetPointers(foundPoint.Transform.position);
            HasDestination = true;
            TargetSet?.Invoke();
        }
    }
}
