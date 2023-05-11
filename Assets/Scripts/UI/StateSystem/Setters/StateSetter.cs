using System;
using UI.StateSystem.Groups;
using UnityEngine;

namespace UI.StateSystem.Setters
{
    public class StateSetter : MonoBehaviour
    {
        private StateType _previousState;
        private StateType _currentState;

        public event Action<StateType> StateSet; 
        public event Action<StatesGroup> GroupClosed;

        public StateType CurrentState => _currentState;

        public StatesGroup CurrentGroup
        {
            get
            {
                if (UIStatesStorage.TryGetStateContainer(_currentState, out StateContainer currentStateContainer) == false)
                    return null;
                
                foreach (var group in UIStatesStorage.StatesGroups)
                {
                    if (group.States.Contains(currentStateContainer.State))
                        return group;
                }

                return null;
            }
        }
        
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.MapView.UIStatesStorage;


        public void SetState(StateType stateType)
        {
            SetState(stateType, out StateContainer stateContainer);
        }
        
        public void SetState(StateType stateType, out StateContainer stateContainer)
        {
            if (TryGetState(stateType, out stateContainer) == false)
                return;

            if (_currentState != stateType)
            {
                _previousState = _currentState;
                _currentState = stateType;   
            }

            foreach (var widget in stateContainer.Widgets)
                widget.SetActive();
            
            CloseState();
            CloseGroup();
            InitializeGroup();
            InitializeState(stateContainer);
            StateSet?.Invoke(stateType);
        }
        
        public void SetPreviousState()
        {
            SetState(_previousState);
        }

        private void InitializeState(StateContainer stateContainer)
        {
            stateContainer.State.OnOpen();
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
                    group.OnOpen();
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
                    GroupClosed?.Invoke(group);
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
            return UIStatesStorage.TryGetStateContainer(stateType, out stateContainer);
        }
    }
}
