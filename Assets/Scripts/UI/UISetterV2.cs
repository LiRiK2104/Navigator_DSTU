using UI.StateSystem;
using UI.StateSystem.Setters;
using UnityEngine;

namespace UI
{
    public class UISetterV2 : MonoBehaviour
    {
        [SerializeField] private UIStatesStorage _uiStatesStorage;
        [SerializeField] private StateSetter _stateSetter;

        public UIStatesStorage UIStatesStorage => _uiStatesStorage;
        public StateSetter StateSetter => _stateSetter;
    }
}
