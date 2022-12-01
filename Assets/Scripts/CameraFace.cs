using UnityEngine;

public class CameraFace : MonoBehaviour
{
    public Camera MinimapCamera { get; set; }
    
    
    public void Update()
    {
        if (MinimapCamera == null)
            return;
        
        transform.LookAt(transform.position + MinimapCamera.transform.rotation * Vector3.forward, MinimapCamera.transform.rotation * Vector3.up);
    }
}
