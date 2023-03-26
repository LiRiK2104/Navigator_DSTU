using UnityEngine;

namespace UI.States.Setters
{
    public class StateSetter : MonoBehaviour
    {
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.UIStatesStorage;
        private UIStatesHistory UIStatesHistory => Global.Instance.UISetterV2.UIStatesHistory;
        
        public void SetState(int index)
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
