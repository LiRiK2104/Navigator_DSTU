
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathPointInfoState : State
    {
        [SerializeField] private PathPointInfoView _pathPointInfoView;

        public PathPointInfoView PathPointInfoView => _pathPointInfoView;
        

        public override void OnOpen()
        {
            _pathPointInfoView.Initialize();
        }

        public override void OnClose() { }
    }
}
