using Map;
using Navigation;
using TargetsSystem.Points;
using UnityEngine;

namespace UI.StateSystem.States
{
    public class PointInfoState : State
    {
        [SerializeField] private PointInfoView _pointInfoView;
        
        private DataBase DataBase => Global.Instance.DataBase;
        private MapPointerSetter MapPointerSetter => Global.Instance.Navigator.MapPointerSetter;
        private MapHandlePanel MapHandlePanel => Global.Instance.UISetterV2.MapHandlePanel;
        
        
        public override void OnOpen()
        {
            MapHandlePanel.MapControllingActive = true;
            MapHandlePanel.SignSelectorActive = false;
        }

        public override void OnClose()
        {
            MapPointerSetter.DeactivateAllPointers();
            MapHandlePanel.MapControllingActive = false;
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
            MapPointerSetter.SetPointer(pointerSetRequest);
        }
    }
}
