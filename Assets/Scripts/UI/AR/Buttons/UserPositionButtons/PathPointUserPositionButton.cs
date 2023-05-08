using UI.StateSystem.Setters;
using UI.StateSystem.States;
using UnityEngine;

namespace UI.AR.Buttons.UserPositionButtons
{
    [RequireComponent(typeof(PathPointStateSetter))]
    public class PathPointUserPositionButton : UserPositionButton
    {
        private PathPointStateSetter _pathPointStateSetter;

        protected override void Initialize()
        {
            base.Initialize();
            _pathPointStateSetter = GetComponent<PathPointStateSetter>();
        }

        protected override void Action()
        {
            _pathPointStateSetter.SetState(UserPositionFinder.UserPosition, ARMain.UserFloorIndex, "Мое местоположение", FillingPathFieldType.Priority);
        }
    }
}
