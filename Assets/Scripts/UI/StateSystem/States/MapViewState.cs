
using Map;

namespace UI.StateSystem.States
{
    public class MapViewState : State
    {
        private MapHandlePanel MapHandlePanel => Global.Instance.UISetterV2.MapHandlePanel;
        
        public override void Initialize()
        {
            MapHandlePanel.MapControllingActive = true;
            MapHandlePanel.SignSelectorActive = true;
        }

        public override void OnClose()
        {
            MapHandlePanel.MapControllingActive = false;
            MapHandlePanel.SignSelectorActive = false;
        }
    }
}
