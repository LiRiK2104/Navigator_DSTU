using UnityEngine;
using UnityEngine.UI;

namespace Map.Hiders
{
    [RequireComponent(typeof(Image))]
    public class ImageHider : Hider
    {
        private Image _image;

        private Image Image
        {
            get
            {
                if (_image == null)
                    _image = GetComponent<Image>();

                return _image;
            }
        }

        public override void Hide()
        {
            Image.enabled = false;
        }

        public override void Show()
        {
            Image.enabled = true;
        }
    }
}
