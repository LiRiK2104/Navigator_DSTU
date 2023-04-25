using Navigation;
using TargetsSystem.Points;
using TMPro;
using UI.StateSystem.Setters;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class PointInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _address;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _defaultIcon;
        [SerializeField] private PathPointButtonStateSetter _pathPointStateSetter;
        
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public virtual void Initialize(PointInfo pointInfo)
        {
            _name.text = pointInfo.Name;
            _address.text = pointInfo.Address.ToString();
            
            _icon.gameObject.SetActive(false);
            _defaultIcon.gameObject.SetActive(true);

            if (DataBase.TryGetPoint(pointInfo, out Point point) == false) 
                return;

            _pathPointStateSetter.Initialize(pointInfo);
            
            if (point.SignCreator.SignPreset.HasIcon)
            {
                _defaultIcon.gameObject.SetActive(false);
                _icon.gameObject.SetActive(true);
                _icon.sprite = point.SignCreator.SignPreset.Icon;   
            }
        }
    }
}
