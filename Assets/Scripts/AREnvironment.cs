using UnityEngine;

public class AREnvironment : MonoBehaviour
{
    [SerializeField] private Transform _signsContainer;

    public Transform SignsContainer => _signsContainer;
}
