using System;
using System.Collections;
using UI.Menus;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Calibrator : MonoBehaviour
{
    private bool _isCalibrated;
    private bool _shouldCalibrate;

    public event Action CalibrationReset;
    public event Action Calibrated;
    
    public bool IsCalibrated => _isCalibrated;
    
    private Button CalibrationButton => Global.Instance.UiSetter.CalibrationMenu.CalibrationButton;
    private Button RecalibrationButton => Global.Instance.UiSetter.TrackingMenu.RecalibrationButton;
    private CalibrationMenu CalibrationMenu => Global.Instance.UiSetter.CalibrationMenu;
    private DataBase DataBase => Global.Instance.DataBase;
    private ARSession ArSession => Global.Instance.ArMain.Session;
    private ARSessionOrigin ArSessionOrigin => Global.Instance.ArMain.SessionOrigin;
    private ARCameraManager ArCameraManager => Global.Instance.ArMain.CameraManager;
    private ARTrackedImageManager ArTrackedImageManager => Global.Instance.ArMain.TrackedImageManager;
    private AREnvironment ArEnvironment => Global.Instance.ArEnvironment;


    private void Awake()
    {
        ARSession.stateChanged += OnAppStartCalibrate;
    }

    private void OnEnable()
    {
        ArTrackedImageManager.trackedImagesChanged += StartCalibration;
        RecalibrationButton.onClick.AddListener(ResetCalibration);
        CalibrationButton.onClick.AddListener(SetShouldCalibrate);
    }

    private void OnDisable()
    {
        ArTrackedImageManager.trackedImagesChanged -= StartCalibration;
        RecalibrationButton.onClick.RemoveListener(ResetCalibration);
        CalibrationButton.onClick.RemoveListener(SetShouldCalibrate);
        ARSession.stateChanged -= OnAppStartCalibrate;
    }


    public void Calibrate(VirtualMarker virtualMarker)
    {
        if (_isCalibrated || _shouldCalibrate == false)
            return;
        
        UpdateOrigin(virtualMarker);
        UpdateMarkerLocation(virtualMarker);
        UpdateEnvironmentLocation(virtualMarker);
        ArEnvironment.gameObject.SetActive(true);
            
        _isCalibrated = true;
        _shouldCalibrate = false;
        Calibrated?.Invoke();
    }
    
    private void OnAppStartCalibrate(ARSessionStateChangedEventArgs obj)
    {
        if (obj.state == ARSessionState.SessionInitializing)
        {
            ARSession.stateChanged -= OnAppStartCalibrate;
            ResetCalibration();
        }
    }
    
    private void ResetCalibration()
    {
        ArSession.Reset();
        ArEnvironment.gameObject.SetActive(false);
        _isCalibrated = false;
        CalibrationReset?.Invoke();
    }

    private void SetShouldCalibrate()
    {
        if (_isCalibrated)
            return;

        StopCoroutine(WaitCalibration());
        StartCoroutine(WaitCalibration());
    }

    private IEnumerator WaitCalibration()
    {
        float time = 5f;
        
        CalibrationMenu.SetCalibrationState();
        _shouldCalibrate = true;
        yield return new WaitForSeconds(time);
        _shouldCalibrate = false;
        CalibrationMenu.SetPointingState();
    }

    private void StartCalibration(ARTrackedImagesChangedEventArgs args)
    {
        if (_isCalibrated || _shouldCalibrate == false)
            return;
        
        Debug.Log("Calibration started!");
        var markerName = GetMarkerName(args);

        if (DataBase.TryGetVirtualMarker(markerName, out VirtualMarker virtualMarker))
        {
            Calibrate(virtualMarker);
            Debug.Log("Calibration successfully!");
        }
        else
        {
            Debug.Log("Calibration failed!");
        }
    }

    private void UpdateOrigin(VirtualMarker virtualMarker)
    {
        var cameraTransform = ArCameraManager.transform;
        ArSessionOrigin.transform.position = virtualMarker.transform.position;
        
        ArCameraManager.transform.position = cameraTransform.position;
        ArCameraManager.transform.rotation = cameraTransform.rotation;
    }

    private void UpdateMarkerLocation(VirtualMarker virtualMarker)
    {
        float offsetDistance = 0.5f;
        Quaternion halfTurn = new Quaternion(0, 180, 0, 0);
        Transform userTransform = ArCameraManager.transform;
        Vector3 offset = userTransform.forward * offsetDistance;

        Quaternion targetRotation = userTransform.rotation * halfTurn;
        targetRotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
        
        virtualMarker.transform.SetPositionAndRotation(userTransform.position + offset, targetRotation);
    }
    
    private void UpdateEnvironmentLocation(VirtualMarker virtualMarker)
    {
        ArEnvironment.transform.position = virtualMarker.transform.TransformPoint(virtualMarker.RelativePosition);
        ArEnvironment.transform.rotation = virtualMarker.transform.rotation * virtualMarker.RelativeRotation;
    }

    private string GetMarkerName(ARTrackedImagesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
            return args.updated[0].referenceImage.name;

        if (args.added.Count > 0)
            return args.added[0].referenceImage.name;
       
        return String.Empty;
    }
}
