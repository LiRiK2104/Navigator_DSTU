using UnityEngine;

namespace Calibration
{
    public class VirtualMarker : MonoBehaviour
    {
        public string Id { get; private set; }

        public void Initialize(string id)
        {
            Id = id;
        }
    }
}
