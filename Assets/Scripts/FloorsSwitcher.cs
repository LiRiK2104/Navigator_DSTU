using System;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

public class FloorsSwitcher : MonoBehaviour
{
    [SerializeField] private BetterToggleGroup _toggleGroup;

    public event Action<int> FloorSwitched;
    
    private AREnvironment AREnvironment => Global.Instance.ArEnvironment;

    
    private void OnEnable()
    {
        _toggleGroup.ToggleChanged += OnToggleChanged;
        _toggleGroup.Initialized += SetFirstFloor;
    }

    private void OnDisable()
    {
        _toggleGroup.ToggleChanged -= OnToggleChanged;
        _toggleGroup.Initialized -= SetFirstFloor;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchFloor(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchFloor(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchFloor(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchFloor(3);
        }
    }


    public void SwitchFloor(int floorIndex)
    {
        ChangeToggle(floorIndex);
    }

    private void ChangeToggle(int floorIndex)
    {
        if (_toggleGroup.TryGetToggle(floorIndex, out Toggle toggle))
            _toggleGroup.SelectToggle(toggle);
    }
    
    private void OnToggleChanged(Toggle toggle)
    {
        if (_toggleGroup.TryGetIndex(toggle, out int index) == false) 
            return;
        
        SelectFloor(index);
        FloorSwitched?.Invoke(index);
    }
    
    private void SelectFloor(int floorIndex)
    {
        var floors = AREnvironment.FirstBuilding.Floors;

        foreach (var floor in floors)
            floor.gameObject.SetActive(false);
        
        floors[floorIndex].gameObject.SetActive(true);
    }

    private void SetFirstFloor()
    {
        _toggleGroup.Initialized -= SetFirstFloor;
        
        const int floorIndex = 1;
        SwitchFloor(floorIndex);
    }
}
