using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.States.Setters
{
    [RequireComponent(typeof(Button))]
    public partial class ButtonStateSetter : ExternalStateSetter
    {
        [SerializeField] private int _index;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetState);
        }
        
        
        private void SetState()
        {
            SetState(_index);
        }

        protected override void UpdateIndex(int removedStateIndex)
        {
            if (_index > removedStateIndex)
                _index--;
            else if (_index == removedStateIndex)
                _index = 0;
        }
    }
    
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
                _origin._index = EditorGUILayout.Popup("State", _origin._index, _origin.UIStatesStorage.GetStatesNames(), EditorStyles.popup);

                serializedObject.ApplyModifiedProperties();
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(SlidingPanelStateSetter), false);
                GUI.enabled = true;
            }
        }
#endif
    }
}
