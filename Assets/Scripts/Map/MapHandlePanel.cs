using UnityEngine;

namespace Map
{
    public class MapHandlePanel : MonoBehaviour
    {
        [SerializeField] private MapControl _mapControl;
        [SerializeField] private SignsSelector _signsSelector;

        public MapControl MapControl => _mapControl;
        public SignsSelector SignsSelector => _signsSelector;
    }
}
