using UnityEngine;

namespace Map.Hiders
{
    [RequireComponent(typeof(Renderer))]
    public class RendererHider : Hider
    {
        private Renderer _renderer;

        private Renderer Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<Renderer>();

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
