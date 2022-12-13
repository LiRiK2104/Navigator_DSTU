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

        
        public List<Button> RecalibrationButtons => _recalibrationButtons;
        public SearchPanel SearchPanel => _searchPanel;
        private Camera FullMapCamera => Global.Instance.CameraContainer.MapCamera;
        public Button FollowButton => _followButton;

        
        private void OnEnable()
        {
            _openFullMapButton.onClick.AddListener(OpenFullMap);
            _closeFullMapButton.onClick.AddListener(CloseFullMap);
            _searchButtons.ForEach(button => button.onClick.AddListener(OpenSearchPanel));
        }

        private void OnDisable()
        {
            _openFullMapButton.onClick.RemoveListener(OpenFullMap);
            _closeFullMapButton.onClick.RemoveListener(CloseFullMap);
            _searchButtons.ForEach(button => button.onClick.RemoveListener(OpenSearchPanel));
        }
        
        private void Start()
        {
            CloseFullMap();
        }

        private void OpenSearchPanel()
        {
            _searchPanel.gameObject.SetActive(true);
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
