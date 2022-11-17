using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MapCalibrator : MonoBehaviour
{
    [SerializeField] private Button _calibrationButton;
    [SerializeField] private DataBase _dataBase;
    [SerializeField] private ARSessionOrigin _arSessionOrigin;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject _environment;
    
    private bool _isCalibrated;

    public event Action ShouldCalibrating;
    public event Action Calibrated;
    
    public bool IsCalibrated => _isCalibrated;


    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += Calibrate;
        _calibrationButton.onClick.AddListener(SetShouldCalibrate);
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= Calibrate;
        _calibrationButton.onClick.RemoveListener(SetShouldCalibrate);
    }

    public void SetShouldCalibrate()
    {
        _environment.SetActive(false);
        _isCalibrated = false;
        ShouldCalibrating?.Invoke();
    }

    private void Calibrate(ARTrackedImagesChangedEventArgs args)
    {
        if (_isCalibrated)
            return;

        Debug.Log("Calibration started!");
        
        SetActiveOnlyTrackingTrackables(args.added);
        SetActiveOnlyTrackingTrackables(args.updated);
        args.removed.ForEach(trackable => trackable.gameObject.SetActive(false));
        
        var markerName = GetMarkerName(args);

        if (TryFindMarker(out MarkerCursor markerCursor) && 
            _dataBase.TryGetPoint(markerName, out Point foundPoint))
        {
            UpdateMarkerLocation(markerCursor, foundPoint.VirtualMarker);
            UpdateEnvironmentLocation(foundPoint.VirtualMarker);
            _environment.SetActive(true);
            
            _isCalibrated = true;
            Calibrated?.Invoke();

            Debug.Log("Calibration successfully!");
        }
        else
        {
            Debug.Log("Calibration failed!");
        }
    }

    private string GetMarkerName(ARTrackedImagesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
            return args.updated[0].referenceImage.name;

        if (args.added.Count > 0)
            return args.added[0].referenceImage.name;
       
        return String.Empty;
    }

    private void SetActiveOnlyTrackingTrackables(List<ARTrackedImage> trackables)
    {
        foreach (var trackable in trackables)
            trackable.gameObject.SetActive(trackable.trackingState != TrackingState.None);
    }

    private bool TryFindMarker(out MarkerCursor markerCursor)
    {
        markerCursor = _arSessionOrigin.GetComponentInChildren<MarkerCursor>();
        return markerCursor != null && markerCursor.gameObject.activeSelf;
    }
    
    private void UpdateMarkerLocation(MarkerCursor markerCursor, VirtualMarker virtualMarker)
    {
        if(markerCursor == null)
            Debug.LogError("markerCursor == null");
        
        Quaternion rotationOffsetX = Quaternion.Euler(90, 0, 0);
        Quaternion rotationOffsetY = Quaternion.Euler(0, -180, 0);
        Quaternion targetRotation = markerCursor.transform.rotation * rotationOffsetX * rotationOffsetY;
        targetRotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
        
        virtualMarker.transform.SetPositionAndRotation(markerCursor.transform.position, targetRotation);
    }
    
    private void UpdateEnvironmentLocation(VirtualMarker virtualMarker)
    {
        _environment.transform.position = virtualMarker.transform.TransformPoint(virtualMarker.RelativePosition);
        _environment.transform.rotation = virtualMarker.transform.rotation * virtualMarker.RelativeRotation;
    }
}
