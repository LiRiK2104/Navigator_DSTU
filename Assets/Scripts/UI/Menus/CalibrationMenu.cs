using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class CalibrationMenu : Menu
    {
        [SerializeField] private Button _calibrationButton;

        public Button CalibrationButton => _calibrationButton;
    }
}
