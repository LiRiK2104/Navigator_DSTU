using TMPro;
using UI.Toggles;
using UnityEngine;
using Toggle = UI.Toggles.Toggle;

namespace UI.FloorsSwitch
{
    [RequireComponent(typeof(Toggle))]
    public class FloorToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _number;


        public void Initialize(int number)
        { 
            _number.text = number.ToString();
        }
    }
}
