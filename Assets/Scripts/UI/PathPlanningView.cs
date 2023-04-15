using UnityEngine;

namespace UI
{
    public class PathPlanningView : MonoBehaviour
    {
        [SerializeField] private FakeInputField _pointAField;
        [SerializeField] private FakeInputField _pointBField;
        [SerializeField] private PathInfoPanel _pathInfoPanel;

        public void SetTextPointAField(string text)
        {
            _pointAField.SetText(text);
        }
        
        public void SetTextPointBField(string text)
        {
            _pointAField.SetText(text);
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
