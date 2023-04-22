using System.Collections;
using Calibration;
using UI;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARValidator : MonoBehaviour
{
    private UISetter UISetter => Global.Instance.UiSetter;
    private Calibrator Calibrator => Global.Instance.Calibrator;
    

    private void OnEnable()
    {
        if (Application.isEditor)
        {
            Debug.Log("Unity editor: AR not supported, Device Not Capable");
            return;
        }
        
        ARSession.stateChanged += OnARSessionStateChanged;
        //Calibrator.Calibrated += SuccessValidation;
        //Calibrator.CalibrationReset += StartMarkersFinding;
    }
    
    private void OnDisable()
    {
        ARSession.stateChanged -= OnARSessionStateChanged;
        //Calibrator.Calibrated -= SuccessValidation;
        //Calibrator.CalibrationReset -= StartMarkersFinding;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs obj)
    {
        switch (ARSession.state)
        {
            case ARSessionState.CheckingAvailability:
                //UISetter.SetState(SessionStates.Loading, "Проверка доступности...");
                Debug.Log("Still Checking Availability...");
                break;
            
            case ARSessionState.NeedsInstall:
                //UISetter.SetState(SessionStates.Loading);
                Debug.Log("Supported, not installed, requesting installation");
                //TODO: Request ARCore services apk installation and install only if user allows
                StartCoroutine(InstallARCoreApp());
                break;
            
            case ARSessionState.Installing:
                //UISetter.SetState(SessionStates.Loading, "Установка данных...");
                Debug.Log("Supported, apk installing");
                StartCoroutine(InstallARCoreApp());
                break;
            
            case ARSessionState.Ready:
                //UISetter.SetState(SessionStates.Loading, "Данные установлены");
                Debug.Log("Supported and installed");
                break;
            
            case ARSessionState.SessionInitializing:
                Debug.Log("Supported, apk installed. SessionInitializing...");
                break;
            
            case ARSessionState.SessionTracking:
                SuccessValidation();
                Debug.Log("Supported, apk installed. SessionTracking...");
                break;
            
            default:
                //UISetter.SetState(SessionStates.Failed, "Устройство не поддерживает AR");
                Debug.Log("Unsupported, Device Not Capable");
                break;
        }
    }

    private void StartMarkersFinding()
    {
        /*if (Calibrator.IsCalibrated == false)
        {
            UISetter.SetState(SessionStates.Calibration, "Найдите маркер и, поместив его в рамку, отклаибруйте");
        }*/
    }

    private void SuccessValidation()
    {
        /*if (Calibrator.IsCalibrated)
        {
            UISetter.SetState(SessionStates.Tracking);
        }*/
    }
     
    private IEnumerator InstallARCoreApp()
    {
       yield return ARSession.Install();
    }
}
