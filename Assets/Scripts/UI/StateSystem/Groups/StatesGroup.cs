using System.Collections.Generic;
using System.Collections.ObjectModel;
using UI.StateSystem.States;
using UnityEngine;

namespace UI.StateSystem.Groups
{
    public abstract class StatesGroup : MonoBehaviour
    {
        [SerializeField] protected List<State> _states = new List<State>();

        public ReadOnlyCollection<State> States => _states.AsReadOnly();

        
        public abstract void OnOpen();
        public abstract void OnClose();
    }
}
