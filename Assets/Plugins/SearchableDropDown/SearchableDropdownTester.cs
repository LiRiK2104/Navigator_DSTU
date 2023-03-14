using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SearchableDropDown))]
public class SearchableDropdownTester : MonoBehaviour
{
    [SerializeField] private List<string> _options = new List<string>();

    private SearchableDropDown _searchableDropDown;
    
    private void Awake()
    {
        _searchableDropDown = GetComponent<SearchableDropDown>();
        _searchableDropDown.Initialize(_options);
    }
}
