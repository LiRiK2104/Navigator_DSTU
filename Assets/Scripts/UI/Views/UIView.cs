using UnityEngine;

namespace UI.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private bool _enable;

        public bool Enable
        {
            get => _enable;

            set
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();

                _canvasGroup.alpha = value ? 1 : 0;
                _canvasGroup.blocksRaycasts = value;
            }
        }
    }
}
