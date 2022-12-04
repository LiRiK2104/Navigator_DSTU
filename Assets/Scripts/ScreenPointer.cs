using System.Collections;
using Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ScreenPointer : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;

    private Camera _camera;
    
    private DestinationSetter DestinationSetter => Global.Instance.Navigator.DestinationSetter;
    private ARCameraManager ArCameraManager => Global.Instance.ArMain.CameraManager;
    private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
    private Calibrator Calibrator => Global.Instance.Calibrator;


    private void Awake()
    {
        _camera = ArCameraManager.GetComponent<Camera>();
        Hide();
    }

    private void OnEnable()
    {
        DestinationSetter.TargetSet += StartPoint;
        Calibrator.Calibrated += StartPoint;
        Calibrator.CalibrationReset += StopPoint;
    }

    private void OnDisable()
    {
        DestinationSetter.TargetSet -= StartPoint;
        Calibrator.Calibrated -= StartPoint;
        Calibrator.CalibrationReset -= StopPoint;
    }

    
    private void StartPoint(TargetPoint targetPoint)
    {
        StartPoint();
    }

    private void StartPoint()
    {
        if (PathFinder.NearestPoint == Vector3.zero || 
            DestinationSetter.HasDestination == false)
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
            var userPosition = ArCameraManager.transform.position + ArCameraManager.transform.forward;
            var direction = PathFinder.NearestPoint - userPosition;
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
