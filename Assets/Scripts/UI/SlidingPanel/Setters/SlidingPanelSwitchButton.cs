using UnityEngine;
using UnityEngine.UI;

namespace UI.SlidingPanel.Setters
{
    [RequireComponent(typeof(Button))]
    public class SlidingPanelSwitchButton : SlidingPanelSwitchObject
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetPanelPosition);
        }
    }
}
