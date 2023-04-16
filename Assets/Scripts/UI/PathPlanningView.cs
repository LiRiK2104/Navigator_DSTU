using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PathPlanningView : MonoBehaviour
    {
        [SerializeField] private FakeInputField _pointAField;
        [SerializeField] private FakeInputField _pointBField;
        [SerializeField] private PathInfoPanel _pathInfoPanel;
        [SerializeField] private Button _swapButton;
        

        public void Initialize(Action swapCallback)
        {
            _swapButton.onClick.RemoveAllListeners();
            _swapButton.onClick.AddListener(swapCallback.Invoke);
        }
        
        public void SetTextPointAField(string text)
        {
            _pointAField.SetText(text);
        }
        
        public void SetTextPointBField(string text)
        {
            _pointBField.SetText(text);
        }

        public void Clear()
        {
            _pointAField.Clear();
            _pointBField.Clear();
        }

        public void SwapFields()
        {
            string pointAText = _pointAField.Text;
            string pointBText = _pointBField.Text;
            
            SetTextPointAField(pointBText);
            SetTextPointBField(pointAText);
        }

        public void InitializePathInfoPanel()
        {
            _pathInfoPanel.Initialize();
        }
    }
}
