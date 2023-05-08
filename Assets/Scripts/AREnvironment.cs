using Map;
using UI.FloorsSwitch;
using UnityEngine;

public class AREnvironment : MonoBehaviour
{
    [SerializeField] private FirstBuilding _firstBuilding;
    [SerializeField] private GameObject _background;

    public FirstBuilding FirstBuilding => _firstBuilding;
    private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;

    
    private void OnEnable()
    {
        UpdateBackgroundHeight(FloorsSwitcher.CurrentFloorIndex);
        FloorsSwitcher.FloorSwitched += UpdateBackgroundHeight;
    }

    private void OnDisable()
    {
        FloorsSwitcher.FloorSwitched -= UpdateBackgroundHeight;
    }

    
    public float GetFloorHeight(int floorIndex)
    {
        return FirstBuilding.Floors[floorIndex].transform.position.y;
    }
    
    private void UpdateBackgroundHeight(int floorIndex)
    {
        var backgroundPosition = _background.transform.position;
        backgroundPosition.y = GetFloorHeight(floorIndex);
        _background.transform.position = backgroundPosition;
    }
}
