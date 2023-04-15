
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathViewState : State
    {
        [SerializeField] private PathInfoPanel _pathInfoPanel;
        
        public override void OnOpen()
        {
            _pathInfoPanel.Initialize();
        }

        public override void OnClose() { }
    }
}
