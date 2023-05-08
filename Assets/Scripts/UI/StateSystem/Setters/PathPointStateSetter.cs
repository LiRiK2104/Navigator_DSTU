using Navigation;
using UI.StateSystem.States;
using UnityEngine;

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
        
        public void SetState(Vector3 position, int floorIndex, string name, FillingPathFieldType fillingPathFieldType)
        {
            SetState(StateType.PathPlanning);

            if (UIStatesStorage.TryGetState(StateType.PathPlanning, out PathPlanningState pathPlanningState))
                pathPlanningState.SetPoint(position, floorIndex, name, fillingPathFieldType);
        }
    }
}
