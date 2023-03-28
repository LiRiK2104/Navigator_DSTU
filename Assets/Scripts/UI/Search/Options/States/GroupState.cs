using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.Search.Options.States
{
    public class GroupState : OptionState
    {
        [SerializeField] private TextMeshProUGUI _name;

        public void Initialize(PointsGroup group)
        {
            InitializeIcon(group.Sprite);
            _name.text = group.Name;
        }
    }
}
