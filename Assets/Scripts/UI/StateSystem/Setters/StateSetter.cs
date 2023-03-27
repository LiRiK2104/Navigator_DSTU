using System;
using UnityEngine;

namespace UI.StateSystem.Setters
{
    public class StateSetter : MonoBehaviour
    {
        private StateType _previousState;
        private StateType _currentState;
        
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.UIStatesStorage;

        
        public void SetState(StateType stateType, out StateContainer stateContainer, Action initializationCallback = null)
        {
            if (TryGetState(stateType, out stateContainer) == false)
                return;
            
            _previousState = _currentState;
            _currentState = stateType;

            foreach (var widget in stateContainer.Widgets)
                widget.GameObject.SetActive(widget.Active);
            
            CloseState();
            CloseGroup();
            InitializeState(stateContainer, initializationCallback);
            InitializeGroup();
        }
        
        public void SetPreviousState()
        {
            SetState(_previousState, out StateContainer stateContainer);
        }

        private void InitializeState(StateContainer stateContainer, Action initializationCallback = null)
        {
            if (initializationCallback != null)
                initializationCallback.Invoke();
            else
                stateContainer.State.Initialize();
        }
        
        private void InitializeGroup()
        {
            if (TryGetState(_previousState, out StateContainer previousStateContainer) == false || 
                TryGetState(_currentState, out StateContainer currentStateContainer) == false)
                return;
            
            foreach (var group in UIStatesStorage.StatesGroups)
            {
                if (group.States.Contains(previousStateContainer.State) == false && 
                    group.States.Contains(currentStateContainer.State))
                {
                    group.Initialize();
                }
            }
        }
        
        private void CloseGroup()
        {
            if (TryGetState(_previousState, out StateContainer previousStateContainer) == false || 
                TryGetState(_currentState, out StateContainer currentStateContainer) == false)
                return;
            
            foreach (var group in UIStatesStorage.StatesGroups)
            {
                if (group.States.Contains(previousStateContainer.State) && 
                    group.States.Contains(currentStateContainer.State) == false)
                {
                    group.OnClose();
                }
            }
        }
        
        private void CloseState()
        {
            if (TryGetState(_previousState, out StateContainer previousStateContainer))
                previousStateContainer.State.OnClose();
        }

        private bool TryGetState(StateType stateType, out StateContainer stateContainer)
        {
            return UIStatesStorage.TryGetState(stateType, out stateContainer);
        }
    }
}
