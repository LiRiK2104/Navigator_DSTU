using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _shouldYRotate;
    
    private Quaternion _targetStartRotation;
    private Vector3 _differentPosition;
    private Quaternion _differentRotation;

    private void Start()
    {
        _targetStartRotation = _target.transform.rotation;
        _differentPosition = transform.position - _target.transform.position;

        if (_shouldYRotate)
            _differentRotation = Quaternion.Inverse(_target.transform.rotation) * transform.rotation;
    }

    private void Update()
    {
        if (_shouldYRotate)
        {
            var rotatedDifferenceVector = (_target.transform.rotation * Quaternion.Inverse(_targetStartRotation)) * _differentPosition;
            transform.position = _target.transform.position + rotatedDifferenceVector;
            
            var targetRotation = _differentRotation.eulerAngles + new Vector3(0, _target.transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(targetRotation);
        }
        else
        {
            transform.position = _target.transform.position + _differentPosition;    
        }
    }
}
