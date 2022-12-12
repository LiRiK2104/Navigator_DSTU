using UnityEngine;

public class CameraFace : MonoBehaviour
{
    private CameraFacePreset _preset;
    

    public void Update()
    {
        if (_preset.Camera == null)
            return;
        
        RotateToCamera();
        Scale();
    }


    public void SetPreset(CameraFacePreset preset)
    {
        _preset = preset;
    }

    private void RotateToCamera()
    {
        var cameraRotation = _preset.Camera.transform.rotation;
        transform.LookAt(transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
    }
 
    private void Scale()
    {
        if (_preset.Camera != null)
        {
            float camHeight;
            if (_preset.Camera.orthographic)
            {
                camHeight = _preset.Camera.orthographicSize * 2;
            }
            else
            {
                float distanceToCamera = Vector3.Distance(_preset.Camera.transform.position, transform.position);
                camHeight = 2.0f * distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (_preset.Camera.fieldOfView * 0.5f));
            }
            float scale = (camHeight / Screen.width) * _preset.ScaleFactor;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
