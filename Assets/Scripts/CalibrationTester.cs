using UnityEngine;

public class CalibrationTester : MonoBehaviour
{
    [SerializeField] private VirtualMarker _virtualMarker;
    [SerializeField] private Calibrator _calibrator;
    
    
    private void Update()
    {
#if UNITY_EDITOR
        if (_virtualMarker != null)
            _calibrator.Calibrate(_virtualMarker);  
#endif
    }
}
