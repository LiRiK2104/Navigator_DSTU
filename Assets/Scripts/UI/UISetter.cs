using System;
using UI.Menus;
using UnityEngine;

namespace UI
{
    public class UISetter : MonoBehaviour
    {
        [SerializeField] private CalibrationMenu _calibrationMenu;
        [SerializeField] private TrackingMenu _trackingMenu;
        [SerializeField] private LoadingMenu _loadingMenu;
        [SerializeField] private FailMenu _failMenu;

        private Menu _activeMenu;

        public CalibrationMenu CalibrationMenu => _calibrationMenu;
        public TrackingMenu TrackingMenu => _trackingMenu;
        public LoadingMenu LoadingMenu => _loadingMenu;
        public FailMenu FailMenu => _failMenu;


        public void SetState(SessionStates state, string text = "")
        {
            DeactivateAll();

            switch (state)
            {
                case SessionStates.Loading:
                    _activeMenu = _loadingMenu;
                    SetLoadingState();
                    break;
            
                case SessionStates.Calibration:
                    _activeMenu = _calibrationMenu;
                    SetCalibrationState();
                    break;
            
                case SessionStates.Tracking:
                    _activeMenu = _trackingMenu;
                    SetTrackingState();
                    break;
            
                case SessionStates.Failed:
                    _activeMenu = _failMenu;
                    SetFailedState();
                    break;
            
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            SetText(text);
        }
    
        private void SetLoadingState()
        {
            _loadingMenu.gameObject.SetActive(true);
        }
    
        private void SetCalibrationState()
        {
            _calibrationMenu.gameObject.SetActive(true);
        }
    
        private void SetTrackingState()
        {
            _trackingMenu.gameObject.SetActive(true);
        }
    
        private void SetFailedState()
        {
            _failMenu.gameObject.SetActive(true);
        }

        private void SetText(string text)
        {
            _activeMenu.TextMeshPro.text = text;
        }

        private void DeactivateAll()
        {
            _calibrationMenu.gameObject.SetActive(false);
            _trackingMenu.gameObject.SetActive(false);
            _loadingMenu.gameObject.SetActive(false);
            _failMenu.gameObject.SetActive(false);
        }
    }

    public enum SessionStates
    {
        Loading,
        Calibration,
        Tracking,
        Failed
    }
}