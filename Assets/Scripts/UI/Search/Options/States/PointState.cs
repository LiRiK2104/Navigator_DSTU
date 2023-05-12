using Navigation;
using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.Search.Options.States
{
    public class PointState : OptionState
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _address;

        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public void Initialize(PointInfo pointInfo, bool isStoryOption)
        {
            Sprite sprite = null;
            
            if (DataBase.TryGetPoint(pointInfo, out Point point) && 
                point.SignCreator.SignPreset.HasIcon)
            {
                sprite = point.SignCreator.SignPreset.Icon;
            }
            
            InitializeIcon(isStoryOption, sprite);
            _name.text = pointInfo.Name;
            _address.text = pointInfo.Address.ToString();
        }
    }
}
