using UnityEngine;

namespace UI.States.Setters
{
    public abstract class ExternalStateSetter : MonoBehaviour
    {
        protected UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.UIStatesStorage;
        private StateSetter StateSetter => Global.Instance.UISetterV2.StateSetter;


        protected virtual void OnEnable()
        {
            UIStatesStorage.StateRemoved += UpdateIndex;
        }

        protected virtual void OnDisable()
        {
            UIStatesStorage.StateRemoved -= UpdateIndex;
        }

        
        protected abstract void UpdateIndex(int removedStateIndex);

        protected void SetState(int index) => StateSetter.SetState(index);
    }
}
