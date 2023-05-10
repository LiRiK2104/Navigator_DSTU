using System.Collections;
using AR;
using Helpers;
using Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ScreenPointer : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;

    private Camera _camera;
    private Canvas _canvas;

    private Canvas Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = _arrow.GetComponentInParent<Canvas>();

            return _canvas;
        }
    }
    private ARMain ARMain => Global.Instance.ArMain;
    private ARCameraManager ArCameraManager => ARMain.CameraManager;
    private Camera UICamera => Global.Instance.CameraContainer.UiCamera;
    private PathFinder PathFinder => Global.Instance.Navigator.PathFinder;
    private Vector3 UserPosition => ArCameraManager.transform.position + ArCameraManager.transform.forward;
    private int UserFloorIndex => ARMain.UserFloorIndex;


    private void Awake()
    {
        _camera = ArCameraManager.GetComponent<Camera>();
        Hide();
    }

    private void OnEnable()
    {
        PathFinder.PathFound += StartPoint;
    }

    private void OnDisable()
    {
        PathFinder.PathFound -= StartPoint;
    }


    private void StartPoint()
    {
        var userPoint = new PathPoint(UserPosition, UserFloorIndex);
        
        if (ARMain.Active == false || 
            PathFinder.GetGuidingPathPoint(userPoint) == Vector3.zero)
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
            var userPoint = new PathPoint(UserPosition, UserFloorIndex);
            var nearestPathPoint = PathFinder.GetGuidingPathPoint(userPoint);
            var direction = nearestPathPoint - UserPosition;
            float toTargetDistance = direction.magnitude;
        
            Ray ray = new Ray(UserPosition, direction);
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
        return UICamera.ScreenToWorldPoint(_camera.WorldToScreenPoint(worldPosition));
    }

    private void SetPosition(Vector3 screenPosition)
    {
        _arrow.transform.position = screenPosition;
    }

    private void RotateTo(Vector3 screenPosition)
    {
        var screenCenter = UICamera.transform.position;
        var rotationDirection = screenPosition - screenCenter;
        var rotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);
        _arrow.transform.rotation = rotation;
    }
}
