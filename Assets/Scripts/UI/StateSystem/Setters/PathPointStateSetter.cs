using Navigation;
using UI.StateSystem.States;

namespace UI.StateSystem.Setters
{
    public class PathPointStateSetter : ExternalStateSetter
    {
        public void SetState(PointInfo pointInfo, FillingPathFieldType fillingPathFieldType)
        {
            SetState(StateType.PathPlanning);

            if (UIStatesStorage.TryGetState(StateType.PathPlanning, out PathPlanningState pathPlanningState))
                pathPlanningState.SetPoint(pointInfo, fillingPathFieldType);
        }
    }
}
