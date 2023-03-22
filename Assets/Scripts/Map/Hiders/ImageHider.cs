using UnityEngine;
using UnityEngine.UI;

namespace Map.Hiders
{
    [RequireComponent(typeof(Image))]
    public class ImageHider : Hider
    {
        private Image _renderer;

        private Image Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<Image>();

                return _renderer;
            }
        }

        public override void Hide()
        {
            Renderer.enabled = false;
        }

        public override void Show()
        {
            Renderer.enabled = true;
        }
    }
}
