using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _shouldYRotate;

    private Vector3 _differentPosition;
    private Quaternion _differentRotation;

    private void Start()
    {
        _differentPosition = transform.position - _target.transform.position;

        if (_shouldYRotate)
            _differentRotation = Quaternion.Inverse(_target.transform.rotation) * transform.rotation;
    }

    private void Update()
    {
        transform.position = _target.transform.position + _differentPosition;

        if (_shouldYRotate)
        {
            var targetRotation = _differentRotation.eulerAngles + new Vector3(0, _target.transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
}
