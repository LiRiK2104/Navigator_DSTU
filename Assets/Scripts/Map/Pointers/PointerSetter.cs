using Navigation;
using UnityEngine;

namespace Map.Pointers
{
    public abstract class PointerSetter : MonoBehaviour
    {
        protected AREnvironment Environment => Global.Instance.ArEnvironment;

        protected abstract void SetPointer(Vector3 targetPosition);
    }
}
