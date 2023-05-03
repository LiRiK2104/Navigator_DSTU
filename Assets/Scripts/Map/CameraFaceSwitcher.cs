using UI;
using UI.Menus;
using UnityEngine;

namespace Map
{
    public class CameraFaceSwitcher : MonoBehaviour
    {
        private const float MinScaleFactor = 0.2f;
        private const float MaxScaleFactor = 5;

        [SerializeField] [Range(MinScaleFactor, MaxScaleFactor)] private float _minimapScaleFactor;
        [SerializeField] [Range(MinScaleFactor, MaxScaleFactor)] private float _mapScaleFactor;

        private Canvas _canvas;
        private CameraFace _cameraFace;
        private CameraFacePreset _minimapPreset;
        private CameraFacePreset _mapPreset;

        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;

        private Canvas Canvas
        {
            get
            {
                if (_canvas == null)
                    _canvas = GetComponentInParent<Canvas>();
            
                return _canvas;
            }
        }
        
        private CameraFace CameraFace
        {
            get
            {
                if (_cameraFace == null)
                    _cameraFace = GetComponent<CameraFace>();
            
                return _cameraFace;
            }
        }
    
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
            SetPresetByViewMode(UISetterV2.CurrentViewMode);
            UISetterV2.ViewSet += SetPresetByViewMode;
        }

        private void OnDisable()
        {
            UISetterV2.ViewSet -= SetPresetByViewMode;
        }


        private void SetPresetByViewMode(ViewMode viewMode)
        {
            switch (viewMode)
            {
                case ViewMode.Map:
                    SetMapPreset();
                    break;
            
                case ViewMode.Worldspace:
                    SetWorldspacePreset();
                    break;
            }
        }

        private void SetMapPreset()
        {
            CameraFace.SetPreset(MapPreset);
            SetCameraToCanvas(MapPreset);
        }
    
        private void SetWorldspacePreset()
        {
            CameraFace.SetPreset(MinimapPreset);
            SetCameraToCanvas(MinimapPreset);
        }

        private void SetCameraToCanvas(CameraFacePreset preset)
        {
            if (Canvas == null)
                return;
            
            Canvas.worldCamera = preset.Camera;
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
}