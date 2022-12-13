using UnityEngine;

public class CameraLineFace : CameraFace
{
    private const float ScaleMultiplier = 15;
    
    [SerializeField] private LineRenderer _lineRenderer;
    
    public override void Update()
    {
        Scale();
    }

    protected override void Scale()
    {
        var scale = CalculateScale();
        _lineRenderer.widthMultiplier = scale * ScaleMultiplier;
    }
    
    protected override float CalculateCameraHeight()
    {
        if (Preset.Camera.orthographic)
            return Preset.Camera.orthographicSize * 2;
        
        float distanceToCamera = 25;
        return 2.0f * distanceToCamera * Mathf.Tan(Mathf.Deg2Rad * (Preset.Camera.fieldOfView * 0.5f));
    }
}
