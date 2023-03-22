using UnityEngine;

namespace Calibration
{
    public class VirtualMarker : MonoBehaviour
    {
        [SerializeField] private string _id; 
        
        public string Id => _id;
    }
}
