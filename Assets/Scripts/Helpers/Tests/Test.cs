using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Helpers.Tests
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private ARSession _arSession;
        [SerializeField] private ARSessionOrigin _arSessionOrigin;
        [SerializeField] private ARValidator _arValidator;

        public void EnableValidator() => SetValidatorEnable(true);
        public void DisableValidator() => SetValidatorEnable(false);
        
        public void EnableArSession() => SetArSessionEnable(true);
        public void DisableArSession() => SetArSessionEnable(false);
        
        public void EnableArSessionOrigin() => SetArSessionOriginEnable(true);
        public void DisableArSessionOrigin() => SetArSessionOriginEnable(false);
        

        private void SetValidatorEnable(bool enable)
        {
            _arValidator.gameObject.SetActive(enable);
        }
        
        private void SetArSessionEnable(bool enable)
        {
            _arSession.gameObject.SetActive(enable);
        }
        
        private void SetArSessionOriginEnable(bool enable)
        {
            _arSessionOrigin.gameObject.SetActive(enable);
        }
    }
}





