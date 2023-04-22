using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARMain : MonoBehaviour
{
    [SerializeField] private ARSession _session;
    [SerializeField] private ARSessionOrigin _sessionOrigin;
    [SerializeField] private ARCameraManager _cameraManager;
    [SerializeField] private ARTrackedImageManager _trackedImageManager;
    [SerializeField] private bool _enabled;
    
    public ARSession Session => _session;
    public ARSessionOrigin SessionOrigin => _sessionOrigin;
    public ARCameraManager CameraManager => _cameraManager;
    public ARTrackedImageManager TrackedImageManager => _trackedImageManager;
    public bool Enabled => _enabled;
    

    public void Exit()
    {
        //TODO: Закрыть все компоненты AR;
    }
}

public interface IARContentUI
{
    public ARMain ARMain { get; }
    
    public void ValidateAREnabled();
}
