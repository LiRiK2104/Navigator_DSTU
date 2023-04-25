using Helpers;
using UnityEngine;

namespace UI.AR.Buttons
{
    public class ViewModeButton : SelfInitializingButton
    {
        [SerializeField] private ViewMode _viewMode;

        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;


        protected override void Action() => SetViewMode();

        private void SetViewMode()
        {
            UISetterV2.SetView(_viewMode);
        }
    }
}
