using UnityEngine;

namespace UI.StateSystem.States
{
    public abstract class State : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void OnClose();
    }
}
