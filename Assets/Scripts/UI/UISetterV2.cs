using UI.States;
using UI.States.Setters;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        [SerializeField] private UIStatesStorage _uiStatesStorage;
        [SerializeField] private UIStatesHistory _uiStatesHistory;
        [SerializeField] private StateSetter _stateSetter;

        public UIStatesStorage UIStatesStorage => _uiStatesStorage;
        public UIStatesHistory UIStatesHistory => _uiStatesHistory;
        public StateSetter StateSetter => _stateSetter;
    }
}
