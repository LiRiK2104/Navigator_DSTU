using UnityEngine;
using UnityEngine.UI;

namespace UI.Search.Options.States
{
    public abstract class OptionState : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultIcon;
        [SerializeField] private GameObject _storyIcon;
        [SerializeField] private Image _icon;
        

        protected void InitializeIcon(bool isStoryOption, Sprite sprite = null)
        {
            _storyIcon.SetActive(false);
            _defaultIcon.SetActive(false);
            _icon.gameObject.SetActive(false);

            if (isStoryOption)
            {
                _storyIcon.SetActive(true);
            }
            else if (sprite == null)
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
