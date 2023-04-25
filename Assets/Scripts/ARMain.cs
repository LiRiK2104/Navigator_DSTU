using System;
using System.Collections;
using Calibration;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARMain : MonoBehaviour
{
    [SerializeField] private ARSession _session;
    [SerializeField] private ARSessionOrigin _sessionOrigin;
    [SerializeField] private ARCameraManager _cameraManager;
    [SerializeField] private ARTrackedImageManager _trackedImageManager;
    [SerializeField] private ARValidator _validator;
    [SerializeField] private Calibrator _calibrator;
    [SerializeField] private bool _available;

    private IEnumerator _enterRoutine;
    
    public event Action Entered;
    public event Action Exited;
    
    public ARSession Session => _session;
    public ARSessionOrigin SessionOrigin => _sessionOrigin;
    public ARCameraManager CameraManager => _cameraManager;
    public ARTrackedImageManager TrackedImageManager => _trackedImageManager;
    public ARValidator Validator => _validator;
    public Calibrator Calibrator => _calibrator;
    public bool Available => _available;
    public bool Active { get; private set; } = false;


    private void Awake()
    {
        EnableSession(false);
    }
    

    public void Enter()
    {
        if (_enterRoutine != null)
            return;
        
        _enterRoutine = EnterRoutine();
        StartCoroutine(_enterRoutine);
    }

    public void Exit()
    {
        Active = false;
        _enterRoutine = null;
        EnableSession(false);
        Exited?.Invoke();
    }
    
    private IEnumerator EnterRoutine()
    {
        if (_available == false)
            yield break;
        
        EnableSession(true);
        yield return Validator.CheckAvailability();

        if (Validator.State != ARValidationState.Completed)
        {
            EnableSession(false);
            yield break;   
        }
        
        yield return Calibrator.GetPreparedCalibrationRoutine();

        if (Calibrator.State != CalibrationState.Completed)
        {
            EnableSession(false);
            yield break;   
        }
        
        Active = true;
        Entered?.Invoke();
    }

    private void EnableSession(bool enabled)
    {
        Session.gameObject.SetActive(enabled);
        SessionOrigin.gameObject.SetActive(enabled);
    }
}

public interface IARContentUI
{
    public ARMain ARMain { get; }
    
    public bool ValidateArAvailable();
}
