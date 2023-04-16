using UI.Search;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathSearchState : State
    {
        [SerializeField] private PathSearchView pathSearchView;

        public PathSearchView PathSearchView => pathSearchView;


        public override void OnOpen()
        {
            pathSearchView.Activate();
        }

        public override void OnClose()
        {
            pathSearchView.Deactivate();
        }
    }
}
