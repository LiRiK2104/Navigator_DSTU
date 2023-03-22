using Helpers;
using UnityEngine;

namespace UI
{
    public abstract class StateSetter : MonoBehaviour
    {
        [SerializeField] protected UIStatesStorage UIStatesStorage; //Global.Instance.UIStatesStorage;


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
            
            state.OnEvent?.Invoke();
        }
    }
}
