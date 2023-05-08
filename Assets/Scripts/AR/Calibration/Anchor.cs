using UnityEngine;

namespace AR.Calibration
{
    public class Anchor : MonoBehaviour
    {
        private Vector3 _relativePosition;
        private Quaternion _relativeRotation;
        
        public Vector3 RelativePosition => _relativePosition;
        public Quaternion RelativeRotation => _relativeRotation;

        private AREnvironment Environment => Global.Instance.ArEnvironment;
    
    
        private void Start()
        {
            SaveStartValues();
        }

    
        private void SaveStartValues()
        {
            _relativePosition = transform.InverseTransformPoint(Environment.transform.position);
            _relativeRotation = Environment.transform.rotation * Quaternion.Inverse(transform.rotation);
        }
    }
}
