using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloorsSwitch
{
    [RequireComponent(typeof(Toggle))]
    public class FloorToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _number;
        
        private Toggle _toggle;


        public void Initialize(ToggleGroup toggleGroup, int number)
        {
            _toggle = GetComponent<Toggle>();
            
            _toggle.group = toggleGroup;
            _number.text = number.ToString();
        }
    }
}
