using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class TrackingMenu : Menu
    {
        [SerializeField] private List<Button> _recalibrationButtons;
        [SerializeField] private List<Button> _searchButtons;
        [SerializeField] private SearchPanel _searchPanel;
        [SerializeField] private GameObject _minimapMode;
        [SerializeField] private GameObject _fullMapMode;
        [SerializeField] private Button _openFullMapButton;
        [SerializeField] private Button _closeFullMapButton;
        [SerializeField] private Button _followButton;
        
        public event Action ToMapSwitched;
        public event Action ToArSwitched;

        public List<Button> RecalibrationButtons => _recalibrationButtons;
        public SearchPanel SearchPanel => _searchPanel;
        private Camera FullMapCamera => Global.Instance.CameraContainer.MapCamera;
        public Button FollowButton => _followButton;
        public ViewMode ViewMode { get; private set; } = ViewMode.Map;

        
        private void OnEnable()
        {
            _openFullMapButton.onClick.AddListener(SwitchToMap);
            _closeFullMapButton.onClick.AddListener(SwitchToAr);
            _searchButtons.ForEach(button => button.onClick.AddListener(OpenSearchPanel));
        }

        private void OnDisable()
        {
            _openFullMapButton.onClick.RemoveListener(SwitchToMap);
            _closeFullMapButton.onClick.RemoveListener(SwitchToAr);
            _searchButtons.ForEach(button => button.onClick.RemoveListener(OpenSearchPanel));
        }
        
        private void Start()
        {
            SwitchToMap();
        }

        private void OpenSearchPanel()
        {
            _searchPanel.gameObject.SetActive(true);
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
