using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _shouldYRotate;
    
    private Quaternion _targetStartRotation;
    private Vector3 _differentPosition;
    private Quaternion _differentRotation;
    private bool _shouldFollow;

    private Camera Camera => Global.Instance.CameraContainer.MapCamera;
    private Button FollowButton => Global.Instance.UiSetter.TrackingMenu.FollowButton;


    private void OnEnable()
    {
        FollowButton.onClick.AddListener(FlyAndFollow);
    }

    private void OnDisable()
    {
        FollowButton.onClick.RemoveListener(FlyAndFollow);
    }
    

    public void StopFollow()
    {
        if (_shouldFollow == false)
            return;
        
        _shouldFollow = false;
    }
    
    private void FlyAndFollow()
    {
        if (_shouldFollow)
            return;
        
        _shouldFollow = true;
        StartCoroutine(FlyAndBind());
    }
    
    private IEnumerator FlyAndBind()
    {
        yield return FlyToTarget();
        Bind();
    }
    
    private void Bind()
    {
        _targetStartRotation = _target.transform.rotation;
        _differentPosition = transform.position - _target.transform.position;
        
        if (_shouldYRotate)
            _differentRotation = Quaternion.Inverse(_target.transform.rotation) * transform.rotation;

        StartCoroutine(Follow());
    }
    
    private IEnumerator FlyToTarget()
    {
        Vector3 targetPosition = new Vector3(_target.transform.position.x, Camera.transform.position.y,
            _target.transform.position.z);
        Quaternion transformRotation = Quaternion.Euler(0, _target.transform.rotation.eulerAngles.y, 0) * Quaternion.Euler(90, 0, 0);

        float speed = 2;
        float minDistance = 0.1f;
        float distance;

        do
        {
            Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, transformRotation, Time.deltaTime * speed);
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, targetPosition, Time.deltaTime * speed);
            
            distance = Vector3.Distance(Camera.transform.position, targetPosition);
            yield return null;
        } 
        while (_shouldFollow && distance > minDistance);
    }

    private IEnumerator Follow()
    {
        while (_shouldFollow)
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
            
            yield return null;
        }
    }
}
