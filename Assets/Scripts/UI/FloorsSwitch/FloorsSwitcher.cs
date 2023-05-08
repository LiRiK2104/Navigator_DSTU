using System.Collections.ObjectModel;
using AR;
using AR.Calibration;
using Map;
using UnityEngine;
using Toggle = UI.Toggles.Toggle;
using ToggleGroup = UI.Toggles.ToggleGroup;

namespace UI.FloorsSwitch
{
    public class FloorsSwitcher : MonoBehaviour
    {
        [SerializeField] private FloorToggle _toggleTemplate; 
        [SerializeField] private ToggleGroup _toggleGroup;

        public delegate void FloorSwitchInfo(int floorIndex);
        public event FloorSwitchInfo FloorSwitched;
    
        public int CurrentFloorIndex { get; private set; }
        private ReadOnlyCollection<Floor> Floors => Global.Instance.ArEnvironment.FirstBuilding.Floors;
        private Calibrator Calibrator => Global.Instance.ArMain.Calibrator;
        private UISetterV2 UISetterV2 => Global.Instance.UISetterV2;
        private ARMain ARMain => Global.Instance.ArMain;
        

        private void OnEnable()
        {
            ChangeToggleToCurrentFloor();
            _toggleGroup.Enabled += ChangeToggleToCurrentFloor;
            _toggleGroup.ToggleChanged += OnToggleChanged;
            Calibrator.Completed += SwitchFloorToUser;
            UISetterV2.ViewSet += SwitchFloorToUser;
        }

        private void OnDisable()
        {
            _toggleGroup.Enabled -= ChangeToggleToCurrentFloor;
            _toggleGroup.ToggleChanged -= OnToggleChanged;
            Calibrator.Completed -= SwitchFloorToUser;
            UISetterV2.ViewSet -= SwitchFloorToUser;
        }

        private void Start()
        {
            Initialize();
        }


        public void SwitchFloor(int floorIndex)
        {
            if (_toggleGroup.gameObject.activeInHierarchy)
                ChangeToggle(floorIndex);
            
            SelectFloor(floorIndex);
        }
        
        private void SwitchFloorToUser(ViewMode viewMode)
        {
            if (viewMode == ViewMode.Worldspace)
                SwitchFloorToUser();
        }
        
        private void SwitchFloorToUser()
        {
            SwitchFloor(ARMain.UserFloorIndex);
        }

        private void Initialize()
        {
            for (int i = 0; i < Floors.Count; i++)
            {
                var number = i + 1;
                CreateToggle(number);
            }
        
            _toggleGroup.Initialize();
            SetFirstFloor();
        }

        private void CreateToggle(int number)
        {
            var toggle = Instantiate(_toggleTemplate, _toggleGroup.transform);
            toggle.Initialize(number);
        }

        private void ChangeToggleToCurrentFloor()
        {
            ChangeToggle(CurrentFloorIndex);
        }

        private void ChangeToggle(int floorIndex)
        {
            if (_toggleGroup.TryGetToggle(floorIndex, out Toggle toggle))
                _toggleGroup.SelectToggle(toggle);
        }
    
        private void OnToggleChanged(Toggle toggle)
        {
            if (_toggleGroup.TryGetIndex(toggle, out int index) == false) 
                return;
            
            SelectFloor(index);
        }
    
        private void SelectFloor(int floorIndex)
        {
            if (floorIndex == CurrentFloorIndex)
                return;
            
            foreach (var floor in Floors)
                floor.gameObject.SetActive(false);
        
            Floors[floorIndex].gameObject.SetActive(true);
            CurrentFloorIndex = floorIndex;
            FloorSwitched?.Invoke(floorIndex);
        }

        private void SetFirstFloor()
        {
            const int floorIndex = 0;
            SwitchFloor(floorIndex);
        }
    }
}
