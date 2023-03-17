using TargetsSystem.Rooms;
using TMPro;
using UnityEngine;

namespace Map.Signs.States
{
    public class SearchResultState : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        
        
        public void Initialize(SignPreset signPreset, AccessibleRoom room = null)
        {
            if (signPreset.HasName)
                _name.text = signPreset.Name;
            else if (room != null)
                _name.text = room.Id;
            else
                _name.text = string.Empty;
        }
    }
}
