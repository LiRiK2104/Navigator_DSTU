using AR;
using UI;
using UI.FloorsSwitch;
using UnityEngine;

namespace Map
{
    public class UserArrow : MonoBehaviour
    {
        [SerializeField] private GameObject _mapArrow;
        [SerializeField] private GameObject _worldspaceArrow;

        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;
        private ARMain ARMain => Global.Instance.ArMain;
        private FloorsSwitcher FloorsSwitcher => Global.Instance.FloorsSwitcher;
        

        private void OnEnable()
        {
            SwitchArrow();
            ARMain.Entered += SwitchArrow;
            ARMain.Exited += DisableArrows;
            FloorsSwitcher.FloorSwitched += SwitchArrow;
            UISetterV2.ViewSet += SwitchArrow;
        }

        private void OnDisable()
        {
            ARMain.Entered -= SwitchArrow;
            ARMain.Exited -= DisableArrows;
            FloorsSwitcher.FloorSwitched -= SwitchArrow;
            UISetterV2.ViewSet -= SwitchArrow;
        }
    

        private void SwitchArrow()
        {
            SwitchArrow(UISetterV2.CurrentViewMode, FloorsSwitcher.CurrentFloorIndex);
        }
        
        private void SwitchArrow(int currentFloorIndex)
        {
            SwitchArrow(UISetterV2.CurrentViewMode, currentFloorIndex);
        }

        private void SwitchArrow(ViewMode viewMode)
        {
            SwitchArrow(viewMode, FloorsSwitcher.CurrentFloorIndex);
        }

        private void SwitchArrow(ViewMode viewMode, int currentFloorIndex)
        {
            DisableArrows();

            if (ARMain.Active == false || 
                ARMain.UserFloorIndex != currentFloorIndex)
                return;
        
            switch (viewMode)
            {
                case ViewMode.Map:
                    _mapArrow.SetActive(true);
                    break;
            
                case ViewMode.Worldspace:
                    _worldspaceArrow.SetActive(true);
                    break;
            }
        }

        private void DisableArrows()
        {
            _mapArrow.SetActive(false);
            _worldspaceArrow.SetActive(false);
        }
    }
}
