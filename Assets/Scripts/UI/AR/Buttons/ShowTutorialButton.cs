using Helpers;

namespace UI.AR.Buttons
{
    public class ShowTutorialButton : SelfInitializingButton
    {
        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;

        protected override void Action() => ShowTutorial();

        private void ShowTutorial()
        {
            UISetterV2.ShowTutorial();
        }
    }
}
