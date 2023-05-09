using System;
using AR;
using AR.Calibration;
using Helpers;
using UI.AR.Views;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        private const string SeenTutorialKey = "seen_tutorial";
        
        [SerializeField] private MapView _mapView;
        [SerializeField] private WorldspaceView _worldspaceView;
        [SerializeField] private CalibrationView _calibrationView;
        [SerializeField] private ARTutorialView _arTutorialView;
        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private ErrorView _errorView;

        public event Action<ViewMode> ViewSet;

        public ViewMode CurrentViewMode { get; private set; } = ViewMode.Map;
        public MapView MapView => _mapView;
        private Camera MapCamera => Global.Instance.CameraContainer.MapCamera;
        private ARMain ARMain => Global.Instance.ArMain;
        private Calibrator Calibrator => ARMain.Calibrator;
        private ARValidator Validator => ARMain.Validator;


        private void OnEnable()
        {
            Validator.Loading += ShowLoading;
            Validator.Failed += HideLoading;
            Validator.Failed += ShowError;
            Validator.Completed += HideLoading;
            Calibrator.Started += SetCalibrationView;
            Calibrator.Completed += SetWorldspaceViewIfCan;
            ARMain.Entered += SetWorldspaceViewIfCan;
            ARMain.Exited += SetMapView;
        }

        private void OnDisable()
        {
            Validator.Loading -= ShowLoading;
            Validator.Failed -= HideLoading;
            Validator.Failed -= ShowError;
            Validator.Completed -= HideLoading;
            Calibrator.Started -= SetCalibrationView;
            Calibrator.Completed -= SetWorldspaceViewIfCan;
            ARMain.Entered -= SetWorldspaceViewIfCan;
            ARMain.Exited -= SetMapView;
        }


        public void Initialize()
        {
            MapView.Initialize();
            SetMapView();
        }

        public void ShowTutorialIfNotSeen()
        {
            if (PlayerPrefs.HasKey(SeenTutorialKey) && 
                PlayerPrefs.GetInt(SeenTutorialKey).ToBool())
                return;

            PlayerPrefs.SetInt(SeenTutorialKey, true.ToInt());
            ShowTutorial();
        }
        
        public void ShowTutorial()
        {
            _arTutorialView.gameObject.SetActive(true);
        }

        public void HideError()
        {
            _errorView.gameObject.SetActive(false);
        }

        public void SetView(ViewMode viewMode)
        {
            _mapView.Enable = false;
            _worldspaceView.Enable = false;
            _calibrationView.Enable = false;
            
            switch (viewMode)
            {
                case ViewMode.Map:
                    MapCamera.gameObject.SetActive(true);
                    _mapView.Enable = true;
                    break;
                
                case ViewMode.Worldspace:
                    MapCamera.gameObject.SetActive(false);
                    _worldspaceView.Enable = true;
                    break;
                
                case ViewMode.Calibration:
                    MapCamera.gameObject.SetActive(false);
                    _calibrationView.Enable = true;
                    break;
            }

            CurrentViewMode = viewMode;
            ViewSet?.Invoke(viewMode);
        }
        
        private void SetCalibrationView()
        {
            SetView(ViewMode.Calibration);
        }
        
        private void SetMapView()
        {
            SetView(ViewMode.Map);
        }
        
        private void SetWorldspaceViewIfCan()
        {
            if (ARMain.ShouldSetWorldspaceView)
                SetView(ViewMode.Worldspace);
            else
                SetMapView();
        }

        private void ShowLoading()
        {
            _loadingView.gameObject.SetActive(true);    
        }
        
        private void HideLoading()
        {
            _loadingView.gameObject.SetActive(false);    
        }
        
        private void ShowError()
        {
            _errorView.gameObject.SetActive(true);   
            _errorView.SetError();
        }
    }
    
    public enum ViewMode
    {
        Map,
        Worldspace,
        Calibration
    }
}
