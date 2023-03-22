using Navigation;
using UnityEngine;

namespace Map.Pointers
{
    public abstract class PointerSetter : MonoBehaviour
    {
        protected AREnvironment Environment => Global.Instance.ArEnvironment;
        private DestinationSetter DestinationSetter => Global.Instance.Navigator.DestinationSetter;
        
        
        private void OnEnable()
        {
            DestinationSetter.TargetSet += SetPointer;
        }

        private void OnDisable()
        {
            DestinationSetter.TargetSet -= SetPointer;
        }

        protected abstract void SetPointer(Vector3 targetPosition);
    }
}
