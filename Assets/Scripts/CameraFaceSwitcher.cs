using System;
using UI.Menus;
using UnityEngine;

public class CameraFaceSwitcher : MonoBehaviour
{
    private const float MinScaleFactor = 1;
    private const float MaxScaleFactor = 5;
    
    [SerializeField] private CameraFace _cameraFace;

    [SerializeField] [Range(MinScaleFactor, MaxScaleFactor)] private float _minimapScaleFactor;
    [SerializeField] [Range(MinScaleFactor, MaxScaleFactor)] private float _mapScaleFactor;
    
    private CameraFacePreset _minimapPreset;
    private CameraFacePreset _mapPreset;

    private TrackingMenu TrackingMenu => Global.Instance.UiSetter.TrackingMenu;
    private CameraFacePreset MinimapPreset
    {
        get
        {
            if (_minimapPreset == null)
                _minimapPreset = new CameraFacePreset(Global.Instance.CameraContainer.MinimapCamera, _minimapScaleFactor);
            
            return _minimapPreset;
        }
    }

    private CameraFacePreset MapPreset 
    {
        get
        {
            if (_mapPreset == null)
                _mapPreset = new CameraFacePreset(Global.Instance.CameraContainer.MapCamera, _mapScaleFactor);
            
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
