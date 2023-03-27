using UnityEngine;
using UnityEngine.UI;

namespace UI.Search.Options.States
{
    public abstract class OptionState : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultIcon;
        [SerializeField] private Image _icon;
        

        protected void InitializeSprite(Sprite sprite = null)
        {
            _defaultIcon.SetActive(false);
            _icon.gameObject.SetActive(false);

            if (sprite == null)
            {
                _defaultIcon.SetActive(true);
            }
            else
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = sprite;
            }
        }
    }
}
