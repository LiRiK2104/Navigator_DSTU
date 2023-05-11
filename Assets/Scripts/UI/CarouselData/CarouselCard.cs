using Helpers;
using Map;
using Navigation;
using TargetsSystem.Points;
using TMPro;
using UI.Search.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CarouselData
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(UIGradient))]
    public class CarouselCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;

        private Button _button;
        private UIGradient _gradient;
        private IOptionInfo _optionInfo;
        
        private SearchResultsSelector SearchResultsSelector => Global.Instance.UISetterV2.MapView.SearchResultsSelector;
        private DataBase DataBase => Global.Instance.DataBase;

        
        public void Initialize(Point point)
        {
            var signPreset = point.SignCreator.SignPreset;

            if (DataBase.TryGetPointInfo(point, out PointInfo pointInfo))
                _optionInfo = pointInfo;
            
            Initialize(signPreset.Icon, signPreset.Name);
        }
        
        public void Initialize(PointsGroup group)
        {
            _optionInfo = group;
            Initialize(group.Sprite, group.Name);
        }
        
        private void Initialize(Sprite icon, string name)
        {
            _icon.sprite = icon;
            _name.text = name;

            _gradient = GetComponent<UIGradient>();
            SetGradient();
            
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Select);
        }

        private void Select()
        {
            SearchResultsSelector.Select(_optionInfo);
        }

        private void SetGradient()
        {
            var spriteRect = _icon.sprite.rect;
            var spriteColor = _icon.sprite.texture.AverageColor(spriteRect);
            _gradient.m_color1 = spriteColor;
            _gradient.m_color2 = Color.white;
        }
    }
}
