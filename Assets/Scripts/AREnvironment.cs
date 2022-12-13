using UnityEngine;

public class AREnvironment : MonoBehaviour
{
    [SerializeField] private Transform _roomSignsContainer;

    public Transform RoomSignsContainer => _roomSignsContainer;
}
