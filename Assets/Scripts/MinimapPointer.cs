using UnityEngine;

public class MinimapPointer : Pointer
{
    [SerializeField] private CameraFace _cameraFace;

    public void Initialize(Camera minimapCamera)
    {
        _cameraFace.MinimapCamera = minimapCamera;
    }
}
