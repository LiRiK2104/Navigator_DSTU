using UnityEngine;

public class CameraFace : MonoBehaviour
{
    protected CameraFacePreset Preset;
    

    public virtual void Update()
    {
        if (Preset.Camera == null)
            return;
        
        RotateToCamera();
        Scale();
    }


    public void SetPreset(CameraFacePreset preset)
    {
        Preset = preset;
    }
    
    protected virtual void Scale()
    {
        var scale = CalculateScale();
        transform.localScale = new Vector3(scale, scale, scale);
    }
    
    protected float CalculateScale()
    {
        if (Preset.Camera == null)
            return 1;

        float scale = (CalculateCameraHeight() / Screen.width) * Preset.ScaleFactor;
        return scale;
    }

    protected virtual float CalculateCameraHeight()
    {
        if (Preset.Camera.orthographic)
            return Preset.Camera.orthographicSize * 2;
        
        float distanceToCamera = Vector3.Distance(Preset.Camera.transform.position, transform.position);
        return 2.0f * distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (Preset.Camera.fieldOfView * 0.5f));
    }

    private void RotateToCamera()
    {
        var cameraRotation = Preset.Camera.transform.rotation;
        transform.LookAt(transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
    }
}
