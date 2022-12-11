using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class TrackingMenu : Menu
    {
        [SerializeField] private Button _recalibrationButton;
        [SerializeField] private SearchableDropDown _targetsDropdown;
        [SerializeField] private GameObject _minimapMode;
        [SerializeField] private GameObject _fullMapMode;
        [SerializeField] private Button _openFullMapButton;
        [SerializeField] private Button _closeFullMapButton;
        [SerializeField] private Button _followButton;

        
        public Button RecalibrationButton => _recalibrationButton;
        public SearchableDropDown TargetsDropdown => _targetsDropdown;
        private Camera FullMapCamera => Global.Instance.CameraContainer.MapCamera;
        public Button FollowButton => _followButton;

        
        private void OnEnable()
        {
            _openFullMapButton.onClick.AddListener(OpenFullMap);
            _closeFullMapButton.onClick.AddListener(CloseFullMap);
        }

        private void OnDisable()
        {
            _openFullMapButton.onClick.RemoveListener(OpenFullMap);
            _closeFullMapButton.onClick.RemoveListener(CloseFullMap);
        }
        
        private void Start()
        {
            CloseFullMap();
        }

        
        private void OpenFullMap()
        {
            FullMapCamera.gameObject.SetActive(true);
            _fullMapMode.SetActive(true);
            _minimapMode.SetActive(false);
        }
        
        private void CloseFullMap()
        {
            FullMapCamera.gameObject.SetActive(false);
            _fullMapMode.SetActive(false);
            _minimapMode.SetActive(true);
        }
    }
}
