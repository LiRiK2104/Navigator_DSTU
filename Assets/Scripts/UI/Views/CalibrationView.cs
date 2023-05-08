
namespace UI.Views
{
    public class CalibrationView : UIView
    {
        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;
        
        private void OnEnable()
        {
            UISetterV2.ShowTutorialIfNotSeen();
        }
    }
}
