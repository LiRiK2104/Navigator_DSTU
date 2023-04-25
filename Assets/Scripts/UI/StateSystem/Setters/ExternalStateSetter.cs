using UnityEngine;

namespace UI.StateSystem.Setters
{
    public abstract class ExternalStateSetter : MonoBehaviour
    {
        protected UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.MapView.UIStatesStorage;
        private StateSetter StateSetter => Global.Instance.UISetterV2.MapView.StateSetter;


        protected virtual void OnEnable()
        {
            UIStatesStorage.StateRemoved += UpdateIndex;
        }

        protected virtual void OnDisable()
        {
            UIStatesStorage.StateRemoved -= UpdateIndex;
        }

        
        protected virtual void UpdateIndex(int removedStateIndex) {}

        protected void SetState(StateType stateType) => StateSetter.SetState(stateType);
        protected void SetPreviousState() => StateSetter.SetPreviousState();
    }
}
