using UnityEngine;

public class VirtualMarker : MonoBehaviour
{
    [SerializeField] private string _id;
    
    private Vector3 _relativePosition;
    private Quaternion _relativeRotation;

    public string Id => _id;
    public Vector3 RelativePosition => _relativePosition;
    public Quaternion RelativeRotation => _relativeRotation;

    private AREnvironment Environment => Global.Instance.ArEnvironment;
    
    
    private void Start()
    {
        SaveStartValues();
    }

    
    private void SaveStartValues()
    {
        _relativePosition = transform.InverseTransformPoint(Environment.transform.position);
        _relativeRotation = Environment.transform.rotation * Quaternion.Inverse(transform.rotation);
    }
}
