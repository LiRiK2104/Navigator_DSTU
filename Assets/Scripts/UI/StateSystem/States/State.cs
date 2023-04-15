using UnityEngine;

namespace UI.StateSystem.States
{
    public abstract class State : MonoBehaviour
    {
        public abstract void OnOpen();
        public abstract void OnClose();
    }
}
