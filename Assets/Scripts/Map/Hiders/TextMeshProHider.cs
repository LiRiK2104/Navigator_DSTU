using TMPro;
using UnityEngine;

namespace Map.Hiders
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProHider : Hider
    {
        private TextMeshProUGUI _renderer;

        private TextMeshProUGUI Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<TextMeshProUGUI>();

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