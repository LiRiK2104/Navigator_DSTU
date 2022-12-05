using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class CalibrationMenu : Menu
    {
        [SerializeField] private Button _calibrationButton;
        [SerializeField] private GameObject _pointingState;
        [SerializeField] private GameObject _calibrationState;

        public Button CalibrationButton => _calibrationButton;

        public void SetPointingState()
        {
            _pointingState.SetActive(true);
            _calibrationState.SetActive(false);
        }
        
        public void SetCalibrationState()
        {
            _calibrationState.SetActive(true);
            _pointingState.SetActive(false);
        }
    }
}
