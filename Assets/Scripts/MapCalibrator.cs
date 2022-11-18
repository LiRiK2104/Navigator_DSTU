using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class MapCalibrator : MonoBehaviour
{
    [SerializeField] private Button _calibrateButton;
    [SerializeField] private Button _recalibrateButton;
    [SerializeField] private DataBase _dataBase;
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject _environment;
    
    private bool _isCalibrated;
    private bool _shouldCalibrate;

    public event Action CalibrationReset;
    public event Action Calibrated;
    
    public bool IsCalibrated => _isCalibrated;


    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += Calibrate;
        _recalibrateButton.onClick.AddListener(ResetCalibration);
        _calibrateButton.onClick.AddListener(SetShouldCalibrate);
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= Calibrate;
        _recalibrateButton.onClick.RemoveListener(ResetCalibration);
        _calibrateButton.onClick.RemoveListener(SetShouldCalibrate);
    }


    public void ResetCalibration()
    {
        _environment.SetActive(false);
        _isCalibrated = false;
        CalibrationReset?.Invoke();
    }

    private void SetShouldCalibrate()
    {
        if (_isCalibrated)
            return;
        
        _shouldCalibrate = true;
    }

    private void Calibrate(ARTrackedImagesChangedEventArgs args)
    {
        if (_isCalibrated || _shouldCalibrate == false)
            return;
        
        Debug.Log("Calibration started!");

        var markerName = GetMarkerName(args);

        if (_dataBase.TryGetPoint(markerName, out Point foundPoint))
        {
            UpdateMarkerLocation(foundPoint.VirtualMarker);
            UpdateEnvironmentLocation(foundPoint.VirtualMarker);
            _environment.SetActive(true);
            
            _isCalibrated = true;
            _shouldCalibrate = false;
            Calibrated?.Invoke();

            Debug.Log("Calibration successfully!");
        }
        else
        {
            Debug.Log("Calibration failed!");
        }
    }

    private void UpdateMarkerLocation(VirtualMarker virtualMarker)
    {
        float offsetDistance = 0.5f;
        Quaternion halfTurn = new Quaternion(0, 180, 0, 0);
        Transform userTransform = _arCameraManager.transform;
        Vector3 offset = userTransform.forward * offsetDistance;

        Quaternion targetRotation = userTransform.rotation * halfTurn;
        targetRotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
        
        virtualMarker.transform.SetPositionAndRotation(userTransform.position + offset, targetRotation);
    }
    
    private void UpdateEnvironmentLocation(VirtualMarker virtualMarker)
    {
        _environment.transform.position = virtualMarker.transform.TransformPoint(virtualMarker.RelativePosition);
        _environment.transform.rotation = virtualMarker.transform.rotation * virtualMarker.RelativeRotation;
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
