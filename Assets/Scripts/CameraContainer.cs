using UnityEngine;

public class CameraContainer : MonoBehaviour
{
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private Camera _minimapCamera;
    [SerializeField] private Camera _uiCamera;

    public Camera MapCamera => _mapCamera;
    public Camera MinimapCamera => _minimapCamera;
    public Camera UiCamera => _uiCamera;
}
