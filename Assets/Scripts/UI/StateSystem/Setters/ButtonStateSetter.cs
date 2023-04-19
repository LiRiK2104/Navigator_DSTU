using UI.SlidingPanel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StateSystem.Setters
{
    [RequireComponent(typeof(Button))]
    public partial class ButtonStateSetter : ExternalStateSetter
    {
        [SerializeField] private StateType _stateType;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetState);
        }
        
        
        private void SetState()
        {
            SetState(_stateType);
        }

        protected override void UpdateIndex(int removedStateIndex)
        {
            if ((int)_stateType > removedStateIndex)
                _stateType--;
            else if ((int)_stateType == removedStateIndex)
                _stateType = 0;
        }
    }
    
    #region Editor
    public partial class ButtonStateSetter
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(ButtonStateSetter))]
        public class ButtonStateSetterEditor : Editor
        {
            private ButtonStateSetter _origin;
            
            private void OnEnable()
            {
                _origin = target as ButtonStateSetter;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                DrawScriptLink();
                _origin._stateType = (StateType)EditorGUILayout.EnumPopup("State", _origin._stateType);

                serializedObject.ApplyModifiedProperties();
                
                if (GUI.changed)
                    EditorUtility.SetDirty(target);
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(SlidingPanelStatesStorage), false);
                GUI.enabled = true;
            }
        }
#endif
    }
    #endregion
}
