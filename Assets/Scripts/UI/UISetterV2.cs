using System;
using Calibration;
using Map;
using UI.AR;
using UI.AR.Views;
using UI.SlidingPanel;
using UI.StateSystem;
using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        [SerializeField] private MapView _mapView;
        [SerializeField] private WorldspaceView _worldspaceView;
        [SerializeField] private CalibrationView _calibrationView;
        [SerializeField] private ARTutorialView _arTutorialView;
        [SerializeField] private LoadingView _loadingView;
        [SerializeField] private ErrorView _errorView;

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
            Calibrator.Completed += SetWorldspaceView;
            ARMain.Entered += SetWorldspaceView;
            ARMain.Exited += SetMapView;
        }

        private void OnDisable()
        {
            Validator.Loading -= ShowLoading;
            Validator.Failed -= HideLoading;
            Validator.Failed -= ShowError;
            Validator.Completed -= HideLoading;
            Calibrator.Started -= SetCalibrationView;
            Calibrator.Completed -= SetWorldspaceView;
            ARMain.Entered -= SetWorldspaceView;
            ARMain.Exited -= SetMapView;
        }


        public void Initialize()
        {
            MapView.Initialize();
            SetMapView();
        }
        
        public void HideError()
        {
            _errorView.gameObject.SetActive(false);
        }
        
        public void ShowTutorial()
        {
            _arTutorialView.gameObject.SetActive(true);
        }

        public void SetView(ViewMode viewMode)
        {
            _mapView.gameObject.SetActive(false);
            _worldspaceView.gameObject.SetActive(false);
            _calibrationView.gameObject.SetActive(false);
            
            switch (viewMode)
            {
                case ViewMode.Map:
                    MapCamera.gameObject.SetActive(true);
                    _mapView.gameObject.SetActive(true);
                    break;
                
                case ViewMode.Worldspace:
                    MapCamera.gameObject.SetActive(false);
                    _worldspaceView.gameObject.SetActive(true);
                    break;
                
                case ViewMode.Calibration:
                    MapCamera.gameObject.SetActive(false);
                    _calibrationView.gameObject.SetActive(true);
                    break;
            }
        }
        
        private void SetCalibrationView()
        {
            SetView(ViewMode.Calibration);
        }
        
        private void SetMapView()
        {
            SetView(ViewMode.Map);
        }
        
        private void SetWorldspaceView()
        {
            SetView(ViewMode.Worldspace);
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
