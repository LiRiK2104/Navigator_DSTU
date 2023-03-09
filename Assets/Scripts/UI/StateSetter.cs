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
        }
    }
    
    /*public partial class StateSetter
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(StateSetter), true)]
        public class StateSetterEditor : Editor
        {
            private StateSetter _origin;
            
            private void OnEnable()
            {
                _origin = target as StateSetter;
            }

            public override void OnInspectorGUI()
            {
                _origin.UIStatesStorage = EditorGUILayout.ObjectField("UI States Storage", _origin.UIStatesStorage, typeof(UIStatesStorage)) as UIStatesStorage;

                if (_origin.UIStatesStorage != null)
                {
                    _origin._index = 
                        EditorGUILayout.Popup("State", _origin._index, _origin.UIStatesStorage.GetStatesNames(), EditorStyles.popup);   
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }*/
}
