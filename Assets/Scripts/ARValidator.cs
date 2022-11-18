using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARValidator : MonoBehaviour
{
    [SerializeField] private UISetter _uiSetter;
    [SerializeField] private MapCalibrator _calibrator;


    private void OnEnable()
    {
        if (Application.isEditor)
        {
            Debug.Log("Unity editor: AR not supported, Device Not Capable");
            return;
        }
        
        ARSession.stateChanged += OnARSessionStateChanged;
        _calibrator.Calibrated += SuccessValidation;
        _calibrator.CalibrationReset += StartMarkersFinding;
    }
    
    private void OnDisable()
    {
        ARSession.stateChanged -= OnARSessionStateChanged;
        _calibrator.Calibrated -= SuccessValidation;
        _calibrator.CalibrationReset -= StartMarkersFinding;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        switch (ARSession.state)
        {
            case ARSessionState.CheckingAvailability:
                _uiSetter.SetState(SessionStates.Loading, "Проверка доступности...");
                Debug.Log("Still Checking Availability...");
                break;
            
            case ARSessionState.NeedsInstall:
                _uiSetter.SetState(SessionStates.Loading);
                Debug.Log("Supported, not installed, requesting installation");
                //TODO: Request ARCore services apk installation and install only if user allows
                StartCoroutine(InstallARCoreApp());
                break;
            
            case ARSessionState.Installing:
                _uiSetter.SetState(SessionStates.Loading, "Установка данных...");
                Debug.Log("Supported, apk installing");
                StartCoroutine(InstallARCoreApp());
                break;
            
            case ARSessionState.Ready:
                _uiSetter.SetState(SessionStates.Loading, "Данные установлены");
                Debug.Log("Supported and installed");
                break;
            
            case ARSessionState.SessionInitializing:
                _calibrator.ResetCalibration();
                Debug.Log("Supported, apk installed. SessionInitializing...");
                break;
            
            case ARSessionState.SessionTracking:
                SuccessValidation();
                Debug.Log("Supported, apk installed. SessionTracking...");
                break;
            
            default:
                _uiSetter.SetState(SessionStates.Failed, "Устройство не поддерживает AR");
                Debug.Log("Unsupported, Device Not Capable");
                break;
        }
    }

    private void StartMarkersFinding()
    {
        if (_calibrator.IsCalibrated == false)
            _uiSetter.SetState(SessionStates.Calibration, "Поиск меток...");
    }

    private void SuccessValidation()
    {
        if (_calibrator.IsCalibrated)
            _uiSetter.SetState(SessionStates.Calibrated);
    }
     
    private IEnumerator InstallARCoreApp()
    {
       yield return ARSession.Install();
    }
}
