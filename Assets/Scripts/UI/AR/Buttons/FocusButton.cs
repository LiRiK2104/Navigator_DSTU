using Followers;
using Helpers;

namespace UI.AR.Buttons
{
    public class FocusButton : SelfInitializingButton
    {
        private SoftARFollower SoftARFollower =>
            Global.Instance.UISetterV2.MapView.MapHandlePanel.MapControl.SoftARFollower;
        
        protected override void Action()
        {
            SoftARFollower.FlyAndFollow();
        }
    }
}
