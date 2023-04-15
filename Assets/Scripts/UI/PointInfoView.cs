using Navigation;
using TargetsSystem.Points;
using UnityEngine;
using TMPro;
using UI.StateSystem.Setters;
using UnityEngine.UI;

namespace UI
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

            if (DataBase.TryGetPoint(pointInfo, out Point point) == false || 
                point.SignCreator.SignPreset.HasIcon == false) 
                return;
            
            _defaultIcon.gameObject.SetActive(false);
            _icon.gameObject.SetActive(true);
            _icon.sprite = point.SignCreator.SignPreset.Icon;
            
            _pathPointStateSetter.Initialize(pointInfo);
        }
    }
}
