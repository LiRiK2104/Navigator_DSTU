using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    [RequireComponent(typeof(Button))]
    public abstract class SelfInitializingButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            Initialize();
        }

        protected abstract void Action();
        
        private void Initialize()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Action);
        }
    }
}
