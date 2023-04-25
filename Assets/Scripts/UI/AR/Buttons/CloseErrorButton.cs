using Helpers;

namespace UI.AR.Buttons
{
    public class CloseErrorButton : SelfInitializingButton
    {
        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;

        
        protected override void Action()
        {
            UISetterV2.HideError();
        }
    }
}
