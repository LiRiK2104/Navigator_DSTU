using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Map.Signs
{
    public class Icon : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;

        public void Initialize(SignPreset signPreset)
        {
            if (signPreset.HasIcon)
                _image.sprite = signPreset.Icon;
            
            if (signPreset.HasName)
                _name.text = signPreset.Name;
            else
                _name.text = string.Empty;
        }
    }
}
