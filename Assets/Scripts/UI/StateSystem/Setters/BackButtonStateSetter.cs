using UnityEngine.UI;

namespace UI.StateSystem.Setters
{
    public class BackButtonStateSetter : ExternalStateSetter
    {
        protected override void UpdateIndex(int removedStateIndex) { }
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetPreviousState);
        }
    }
}
