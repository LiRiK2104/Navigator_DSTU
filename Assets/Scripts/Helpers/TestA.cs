using UnityEngine;
using UnityEditor;

namespace Helpers
{
    public partial class TestA : Test
    {
        
    }

    public partial class TestA
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(TestA))]
        public class TestAEditor : Editor
        {
            private TestA _origin;

            private void OnEnable()
            {
                _origin = target as TestA;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();

                DrawScriptLink();
                

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(TestA), false);
                GUI.enabled = true;
            }
        }
#endif

    }
}
