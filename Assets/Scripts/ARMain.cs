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
    [SerializeField] private UserPositionFinder _userPositionFinder;
    [SerializeField] private bool _available;

    private IEnumerator _enterRoutine;
    private int _userFloorIndex;
    
    public event Action Entered;
    public event Action Exited;
    
    public ARSession Session => _session;
    public ARSessionOrigin SessionOrigin => _sessionOrigin;
    public ARCameraManager CameraManager => _cameraManager;
    public ARTrackedImageManager TrackedImageManager => _trackedImageManager;
    public ARValidator Validator => _validator;
    public Calibrator Calibrator => _calibrator;
    public UserPositionFinder UserPositionFinder => _userPositionFinder;
    public bool Available => _available;
    public bool Active { get; private set; } = false;
    public bool ShouldSetWorldspaceView { get; private set; } = true;
    public int UserFloorIndex => _userFloorIndex;


    private void Awake()
    {
        DisableSession();
    }

    private void OnEnable()
    {
        Calibrator.MarkerFound += SetUserFloorIndex;
    }

    private void OnDisable()
    {
        Calibrator.MarkerFound -= SetUserFloorIndex;
    }


    public void Enter(bool shouldSetWorldspaceView = true)
    {
        if (_enterRoutine != null)
            return;

        ShouldSetWorldspaceView = shouldSetWorldspaceView;
        _enterRoutine = EnterRoutine();
        StartCoroutine(_enterRoutine);
    }

    public void Exit()
    {
        Active = false;
        _enterRoutine = null;
        DisableSession();
        Debug.Log("AR session ended.");
        Exited?.Invoke();
    }
    
    private IEnumerator EnterRoutine()
    {
        if (_available == false)
            yield break;
        
        EnableSession();
        yield return Validator.CheckAvailability();

        if (Validator.State != ARValidationState.Completed)
        {
            DisableSession();
            yield break;   
        }
        
        yield return Calibrator.GetPreparedCalibrationRoutine();

        if (Calibrator.State != CalibrationState.Completed)
        {
            DisableSession();
            yield break;   
        }
        
        Debug.Log("AR session started.");
        Active = true;
        Entered?.Invoke();
        ShouldSetWorldspaceView = true;
    }

    private void EnableSession()
    {
        Session.gameObject.SetActive(true);
        SessionOrigin.gameObject.SetActive(true);
    }
    
    private void DisableSession()
    {
        Session.gameObject.SetActive(false);
        SessionOrigin.gameObject.SetActive(false);
    }

    private void SetUserFloorIndex(int floorIndex)
    {
        _userFloorIndex = floorIndex;
    }
}

public interface IARContentUI
{
    public ARMain ARMain { get; }
    
    public bool ValidateArAvailable();
}
