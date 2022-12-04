using TMPro;
using UnityEngine;

namespace UI.Menus
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        public TextMeshProUGUI TextMeshPro => _textMeshPro;
    }
}
