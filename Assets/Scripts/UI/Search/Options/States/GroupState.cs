using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.Search.Options.States
{
    public class GroupState : OptionState
    {
        [SerializeField] private TextMeshProUGUI _name;

        public void Initialize(PointsGroup group, bool isStoryOption)
        {
            InitializeIcon(isStoryOption, group.Sprite);
            _name.text = group.Name;
        }
    }
}
