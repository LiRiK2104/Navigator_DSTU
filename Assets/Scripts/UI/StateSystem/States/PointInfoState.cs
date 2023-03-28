using Map;
using Navigation;
using TargetsSystem.Points;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StateSystem.States
{
    public class PointInfoState : State
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _address;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _defaultIcon;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;
        private MapHandlePanel MapHandlePanel => Global.Instance.MapHandlePanel;
        
        
        public override void Initialize()
        {
            _icon.gameObject.SetActive(false);
            _defaultIcon.gameObject.SetActive(true);
            
            MapHandlePanel.MapControllingActive = true;
            MapHandlePanel.SignSelectorActive = false;
        }

        public override void OnClose()
        {
            MapPointerSetter.DeactivateAllPointers();
            MapHandlePanel.MapControllingActive = false;
        }
        
        public void Initialize(PointInfo pointInfo)
        {
            _name.text = pointInfo.Name;
            _address.text = pointInfo.Address.ToString();
            
            Initialize();

            if (DataBase.TryGetPoint(pointInfo, out Point point) && 
                point.SignCreator.SignPreset.HasIcon)
            {
                _defaultIcon.gameObject.SetActive(false);
                _icon.gameObject.SetActive(true);
                _icon.sprite = point.SignCreator.SignPreset.Icon;
                SetPointer(point);
            }
        }

        private void SetPointer(Point point)
        {
            var pointerSetRequest = new PointerSetRequest(point.transform.position, PointerState.Default);
            MapPointerSetter.SetPointer(pointerSetRequest);
        }
    }
}
