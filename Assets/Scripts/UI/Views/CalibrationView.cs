using UnityEngine;

namespace UI.Views
{
    public class CalibrationView : MonoBehaviour
    {
        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;
        
        private void OnEnable()
        {
            UISetterV2.ShowTutorialIfNotSeen();
        }
    }
}
