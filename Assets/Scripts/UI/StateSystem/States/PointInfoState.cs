using Map;
using Navigation;
using TargetsSystem.Points;
using UI.Views;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PointInfoState : State
    {
        [SerializeField] private PointInfoView _pointInfoView;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;


        public override void OnOpen() { }

        public override void OnClose()
        {
            HidePointer();
        }
        
        public void Initialize(PointInfo pointInfo)
        {
            OnOpen(); 
            _pointInfoView.Initialize(pointInfo);
            
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
