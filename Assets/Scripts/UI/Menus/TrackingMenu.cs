using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class TrackingMenu : Menu
    {
        [SerializeField] private Button _recalibrationButton;
        [SerializeField] private SearchableDropDown _targetsDropdown;
        
        public Button RecalibrationButton => _recalibrationButton;
        public SearchableDropDown TargetsDropdown => _targetsDropdown;
    }
}
