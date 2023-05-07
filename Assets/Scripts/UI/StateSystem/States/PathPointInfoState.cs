using Map;
using Navigation;
using TargetsSystem.Points;
using UI.Views;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PathPointInfoState : State
    {
        [SerializeField] private PathPointInfoView _pathPointInfoView;

        public PathPointInfoView PathPointInfoView => _pathPointInfoView;
        private DataBase DataBase => Global.Instance.DataBase;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;
        

        public override void OnOpen()
        {
            _pathPointInfoView.Initialize();
        }

        public override void OnClose()
        {
            HidePointer();
        }
        
        public void Initialize(PointInfo pointInfo)
        {
            OnOpen(); 
            _pathPointInfoView.Initialize(pointInfo);
            
            if (DataBase.TryGetPoint(pointInfo, out Point point)) 
                SetPointer(point);
        }

        private void SetPointer(Point point)
        {
            var pointerSetRequest = new PointerSetRequest(point.transform.position, PointerState.Default);
            MapPointerSetter.SetPointerAtCurrentFloor(pointerSetRequest);
        }

        private void HidePointer()
        {
            MapPointerSetter.HidePointers(true, PointerState.Default);
        }
    }
}
