using System.Collections.Generic;
using UI.Search;
using UnityEngine;

public class Entry : MonoBehaviour
{
    [SerializeField] private List<SearchableDropDown> _searchableDropDowns = new List<SearchableDropDown>();

    private DataBase DataBase => Global.Instance.DataBase;
    
    private void Start()
    {
        DataBase.Initialize();

        foreach (var point in DataBase.GetAllPoints())
            point.Initialize();

        foreach (var searchableDropDown in _searchableDropDowns)
            searchableDropDown.Initialize();
    }
}
