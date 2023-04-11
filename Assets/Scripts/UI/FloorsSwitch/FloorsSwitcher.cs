using System;
using System.Collections.ObjectModel;
using Helpers;
using Map;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloorsSwitch
{
    [RequireComponent(typeof(BetterToggleGroup))]
    public class FloorsSwitcher : MonoBehaviour
    {
        [SerializeField] private FloorToggle _toggleTemplate;
    
        private BetterToggleGroup _toggleGroup;

        public event Action<int> FloorSwitched;
    
        private ReadOnlyCollection<Floor> Floors => Global.Instance.ArEnvironment.FirstBuilding.Floors;


        private void Awake()
        {
            _toggleGroup = GetComponent<BetterToggleGroup>();
        }

        private void OnEnable()
        {
            _toggleGroup.ToggleChanged += OnToggleChanged;
        }

        private void OnDisable()
        {
            _toggleGroup.ToggleChanged -= OnToggleChanged;
        }

        private void Start()
        {
            Initialize();
        }


        public void SwitchFloor(int floorIndex)
        {
            ChangeToggle(floorIndex);
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
            var toggle = Instantiate(_toggleTemplate, transform);
            toggle.Initialize(_toggleGroup, number);
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
            FloorSwitched?.Invoke(index);
        }
    
        private void SelectFloor(int floorIndex)
        {
            foreach (var floor in Floors)
                floor.gameObject.SetActive(false);
        
            Floors[floorIndex].gameObject.SetActive(true);
        }

        private void SetFirstFloor()
        {
            const int floorIndex = 0;
            SwitchFloor(floorIndex);
        }
    }
}
