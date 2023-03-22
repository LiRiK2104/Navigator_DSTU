using System;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public partial class Test : MonoBehaviour
    {
        
    }

    public partial class Test
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(Test))]
        public class TestEditor : Editor
        {
            private Test _origin;

            private void OnEnable()
            {
                _origin = target as Test;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DrawScriptLink();
                

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(Test), false);
                GUI.enabled = true;
            }
        }
#endif

    }
}





