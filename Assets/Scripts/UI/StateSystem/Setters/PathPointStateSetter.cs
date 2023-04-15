using Navigation;
using UI.StateSystem.States;

namespace UI.StateSystem.Setters
{
    public class PathPointStateSetter : ExternalStateSetter
    {
        public void SetState(PointInfo pointInfo, FillingPathFieldType fillingPathFieldType)
        {
            SetState(StateType.PathPlanning);

            if (UIStatesStorage.TryGetState(StateType.PathPlanning, out StateContainer pathPlanningStateContainer) == false)
                return;

            var pathPlanningState = pathPlanningStateContainer.State as PathPlanningState;

            if (pathPlanningState == null)
                return;
            
            pathPlanningState.SetPoint(pointInfo, fillingPathFieldType);
        }
    }
}
