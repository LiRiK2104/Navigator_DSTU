using Navigation;
using TargetsSystem.Points;
using TMPro;
using UnityEngine;

namespace UI.SearchableDropDown.Options.States
{
    public class PointState : OptionState
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _address;

        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public void Initialize(PointInfo pointInfo)
        {
            Sprite sprite = null;
            
            if (DataBase.TryGetPoint(pointInfo, out Point point) && 
                point.SignCreator.SignPreset.HasIcon)
            {
                sprite = point.SignCreator.SignPreset.Icon;
            }
            
            InitializeSprite(sprite);
            _name.text = pointInfo.Name;
            _address.text = pointInfo.Address.ToString();
        }
    }
}
