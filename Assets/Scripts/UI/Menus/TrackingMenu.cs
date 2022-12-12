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

        public event Action ToMapSwitched;
        public event Action ToArSwitched;

        public Button RecalibrationButton => _recalibrationButton;
        public SearchableDropDown TargetsDropdown => _targetsDropdown;
        private Camera FullMapCamera => Global.Instance.CameraContainer.MapCamera;
        public Button FollowButton => _followButton;
        public ViewMode ViewMode { get; private set; } = ViewMode.Map;

        
        private void OnEnable()
        {
            _openFullMapButton.onClick.AddListener(SwitchToMap);
            _closeFullMapButton.onClick.AddListener(SwitchToAr);
        }

        private void OnDisable()
        {
            _openFullMapButton.onClick.RemoveListener(SwitchToMap);
            _closeFullMapButton.onClick.RemoveListener(SwitchToAr);
        }
        
        private void Start()
        {
            SwitchToMap();
        }

        
        private void SwitchToMap()
        {
            ViewMode = ViewMode.Map;
            FullMapCamera.gameObject.SetActive(true);
            _fullMapMode.SetActive(true);
            _minimapMode.SetActive(false);
            ToMapSwitched?.Invoke();
        }
        
        private void SwitchToAr()
        {
            ViewMode = ViewMode.AR;
            FullMapCamera.gameObject.SetActive(false);
            _fullMapMode.SetActive(false);
            _minimapMode.SetActive(true);
            ToArSwitched?.Invoke();
        }
    }

    public enum ViewMode
    {
        Map,
        AR
    }
}
