using UnityEngine;

namespace Map
{
    public class MapHandlePanel : MonoBehaviour
    {
        [SerializeField] private MapControl _mapControl;
        [SerializeField] private SignsSelector _signsSelector;

        public MapControl MapControl => _mapControl;
        public SignsSelector SignsSelector => _signsSelector;

        public bool MapControllingActive
        {
            get => _mapControl.enabled;
            set => _mapControl.enabled = value;
        }
        
        public bool SignSelectorActive
        {
            get => _signsSelector.enabled;
            set => _signsSelector.enabled = value;
        }
    }
}
