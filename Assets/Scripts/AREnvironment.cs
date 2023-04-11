using Map;
using UnityEngine;

public class AREnvironment : MonoBehaviour
{
    [SerializeField] private FirstBuilding _firstBuilding;

    public FirstBuilding FirstBuilding => _firstBuilding;
}
