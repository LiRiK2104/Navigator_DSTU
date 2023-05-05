using Map;
using UI.StateSystem.States;
namespace UI.StateSystem.Groups
{
    public class NavigationGroup : StatesGroup
    {
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.MapView.UIStatesStorage;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;
        
        
        public override void OnOpen() { }

        public override void OnClose()
        {
            if (UIStatesStorage.TryGetState(StateType.PathPlanning, out PathPlanningState pathPlanningState))
                pathPlanningState.Clear();
            
            MapPointerSetter.HidePointers(true, PointerState.PointA, PointerState.PointB);
        }
    }
}
