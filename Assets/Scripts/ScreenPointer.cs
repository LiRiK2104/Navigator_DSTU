using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ScreenPointer : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;
    [SerializeField] private PathFinder _pathFinder;
    [SerializeField] private DestinationSetter _destinationSetter;
    [SerializeField] private ARCameraManager _cameraManager;
    [SerializeField] private Calibrator _calibrator;

    private Camera _camera;


    private void Awake()
    {
        _camera = _cameraManager.GetComponent<Camera>();
        Hide();
    }

    private void OnEnable()
    {
        _destinationSetter.TargetSet += StartPoint;
        _calibrator.Calibrated += StartPoint;
        _calibrator.CalibrationReset += StopPoint;
    }

    private void OnDisable()
    {
        _destinationSetter.TargetSet -= StartPoint;
        _calibrator.Calibrated -= StartPoint;
        _calibrator.CalibrationReset -= StopPoint;
    }


    private void StartPoint()
    {
        if (_pathFinder.NearestPoint == Vector3.zero || 
            _destinationSetter.HasDestination == false)
            return;
        
        StopPoint();
        Show();
        StartCoroutine(UpdateArrow());
    }

    private void StopPoint()
    {
        StopCoroutine(UpdateArrow());
        Hide();
    }

    private IEnumerator UpdateArrow()
    {
        while (true)
        {
            var userPosition = _cameraManager.transform.position + _cameraManager.transform.forward;
            var direction = _pathFinder.NearestPoint - userPosition;
            float toTargetDistance = direction.magnitude;
        
            Ray ray = new Ray(userPosition, direction);
            float rayMinDistance = GetInCameraViewDistance(ray, direction);

            if (toTargetDistance > rayMinDistance)
                Show();
            else
                Hide();
        
            Vector3 screenPosition = GetScreenPosition(ray, rayMinDistance);
            SetPosition(screenPosition);
            RotateTo(screenPosition);
            yield return null;
        }
    }

    private void Show()
    {
        _arrow.gameObject.SetActive(true);   
    }
    
    private void Hide()
    {
        _arrow.gameObject.SetActive(false);
    }

    private float GetInCameraViewDistance(Ray ray, Vector3 direction)
    {
        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);
        float rayMinDistance = Mathf.Infinity;
        
        foreach (var plane in cameraPlanes)
        {
            if (plane.Raycast(ray, out float toPlaneDistance))
            {
                if (toPlaneDistance < rayMinDistance)
                    rayMinDistance = toPlaneDistance;
            }
        }

        return Mathf.Clamp(rayMinDistance, 0, direction.magnitude);
    }

    private Vector3 GetScreenPosition(Ray ray, float rayDistance)
    {
        Vector3 worldPosition = ray.GetPoint(rayDistance);
        return _camera.WorldToScreenPoint(worldPosition);
    }

    private void SetPosition(Vector3 screenPosition)
    {
        _arrow.transform.position = screenPosition;
    }

    private void RotateTo(Vector3 screenPosition)
    {
        var screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        var rotationDirection = screenPosition - screenCenter;
        var rotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);
        _arrow.transform.rotation = rotation;
    }
}
