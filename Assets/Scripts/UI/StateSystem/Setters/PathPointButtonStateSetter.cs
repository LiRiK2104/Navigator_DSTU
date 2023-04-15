using Navigation;
using UI.StateSystem.States;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UI.StateSystem.Setters
{
    public partial class PathPointButtonStateSetter : PathPointStateSetter
    {
        private Button _button;
        private PointInfo _pointInfo;
        private FillingPathFieldType _fillingPathFieldType;


        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SetState);
        }

        protected override void UpdateIndex(int removedStateIndex)
        {
            if ((int)StateType.PathPlanning == removedStateIndex)
                Debug.LogError("Can't find \"PathPlanning\" state!");
        }


        public void Initialize(PointInfo pointInfo)
        {
            _pointInfo = pointInfo;
        }
        
        private void SetState()
        {
            SetState(_pointInfo, _fillingPathFieldType);
        }
    }

    #region Editor

    public partial class PathPointButtonStateSetter
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(PathPointButtonStateSetter))]
        public class PathPointButtonStateSetterEditor : Editor
        {
            private PathPointButtonStateSetter _origin;

            private void OnEnable()
            {
                _origin = target as PathPointButtonStateSetter;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DrawScriptLink();
                _origin._fillingPathFieldType = 
                    (FillingPathFieldType)EditorGUILayout.EnumPopup("Filling field type", _origin._fillingPathFieldType);

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(PathPointButtonStateSetter), false);
                GUI.enabled = true;
            }
        }
#endif
    }

    #endregion
}
