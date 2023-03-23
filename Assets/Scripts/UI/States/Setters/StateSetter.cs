using UnityEngine;

namespace UI.States.Setters
{
    public abstract class StateSetter : MonoBehaviour
    {
        [SerializeField] protected UIStatesStorage UIStatesStorage; //Global.Instance.UIStatesStorage;
        [SerializeField] protected UIStatesHistory UIStatesHistory;


        protected virtual void OnEnable()
        {
            UIStatesStorage.StateRemoved += UpdateIndex;
        }

        protected virtual void OnDisable()
        {
            UIStatesStorage.StateRemoved -= UpdateIndex;
        }

        
        protected abstract void UpdateIndex(int removedStateIndex);

        protected void SetState(int index)
        {
            if (UIStatesStorage.TryGetState(index, out UIState state) == false)
                return;

            foreach (var widget in state.Widgets)
                widget.GameObject.SetActive(widget.Active);
            
            UIStatesHistory.AddState(index);
            state.OnEvent?.Invoke();
        }
    }
}
