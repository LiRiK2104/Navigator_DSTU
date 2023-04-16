using UI.StateSystem.States;
namespace UI.StateSystem.Groups
{
    public class NavigationGroup : StatesGroup
    {
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.UIStatesStorage;
        
        public override void OnOpen() { }

        public override void OnClose()
        {
            if (UIStatesStorage.TryGetState(StateType.PathPlanning, out PathPlanningState pathPlanningState))
            {
                pathPlanningState.Clear();
            }
        }
    }
}
