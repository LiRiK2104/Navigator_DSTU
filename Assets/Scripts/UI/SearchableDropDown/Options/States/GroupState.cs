using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.SearchableDropDown.Options.States
{
    public class GroupState : OptionState
    {
        [SerializeField] private TextMeshProUGUI _name;

        public void Initialize(PointsGroup group, Sprite sprite = null)
        {
            InitializeSprite(sprite);
            _name.text = group.Name;
        }
    }
}
