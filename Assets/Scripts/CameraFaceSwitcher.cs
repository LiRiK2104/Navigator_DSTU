using System;
using UI.Menus;
using UnityEngine;

public class CameraFaceSwitcher : MonoBehaviour
{
    [SerializeField] private CameraFace _cameraFace;
    
    private CameraFacePreset _minimapPreset;
    private CameraFacePreset _mapPreset;

    private TrackingMenu TrackingMenu => Global.Instance.UiSetter.TrackingMenu;
    private CameraFacePreset MinimapPreset
    {
        get
        {
            if (_minimapPreset == null)
                _minimapPreset = new CameraFacePreset(Global.Instance.CameraContainer.MinimapCamera, 4);
            
            return _minimapPreset;
        }
    }

    private CameraFacePreset MapPreset 
    {
        get
        {
            if (_mapPreset == null)
                _mapPreset = new CameraFacePreset(Global.Instance.CameraContainer.MapCamera, 2);
            
            return _mapPreset;
        }
    }
    

    private void OnEnable()
    {
        TrackingMenu.ToArSwitched += SetMinimapPreset;
        TrackingMenu.ToMapSwitched += SetMapPreset;
    }

    private void OnDisable()
    {
        TrackingMenu.ToArSwitched -= SetMinimapPreset;
        TrackingMenu.ToMapSwitched -= SetMapPreset;
    }

    private void Start()
    {
        SetPresetByViewMode();
    }


    private void SetPresetByViewMode()
    {
        switch (TrackingMenu.ViewMode)
        {
            case ViewMode.Map:
                SetMapPreset();
                break;
            
            case ViewMode.AR:
                SetMinimapPreset();
                break;
        }
    }

    private void SetMapPreset()
    {
        _cameraFace.SetPreset(MapPreset);
    }
    
    private void SetMinimapPreset()
    {
        _cameraFace.SetPreset(MinimapPreset);
    }
}


public class CameraFacePreset
{
    public Camera Camera { get; }
    public float ScaleFactor { get; }

    public CameraFacePreset(Camera camera, float scaleFactor)
    {
        Camera = camera;
        ScaleFactor = scaleFactor;
    }
}
