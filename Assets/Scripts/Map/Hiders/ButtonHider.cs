using UnityEngine;
using UnityEngine.UI;

namespace Map.Hiders
{
    [RequireComponent(typeof(Button))]
    public class ButtonHider : Hider
    {
        private Button _button;

        private Button Button
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();

                return _button;
            }
        }
        
        public override void Hide()
        {
            Button.enabled = false;
        }

        public override void Show()
        {
            Button.enabled = true;
        }
    }
}
