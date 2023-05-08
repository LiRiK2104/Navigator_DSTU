using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace AR.Calibration
{
    [CreateAssetMenu(menuName = nameof(TriadMarkersLibrary), fileName = nameof(TriadMarkersLibrary), order = 51)]
    public partial class TriadMarkersLibrary : ScriptableObject
    {
        [SerializeField] private GameObject _redSpherePrefab;
        [SerializeField] private GameObject _greenSpherePrefab;
        [SerializeField] private GameObject _blueSpherePrefab;
        [SerializeField] private float _width;
        [SerializeField] private List<TriadMarkerData> _triadMarkers = new List<TriadMarkerData>();

        public ReadOnlyCollection<TriadMarkerData> TriadMarkers => _triadMarkers.AsReadOnly();
        public GameObject RedSpherePrefab => _redSpherePrefab;
        public GameObject GreenSpherePrefab => _greenSpherePrefab;
        public GameObject BlueSpherePrefab => _blueSpherePrefab;
        public float Width => _width;
    }

    [Serializable]
    public class TriadMarkerData
    {
        public string Name;
        public Texture2D MarkerA;
        public Texture2D MarkerB;
        public Texture2D MarkerC;

        public string NameMarkerA => $"{Name}a";
        public string NameMarkerB => $"{Name}b";
        public string NameMarkerC => $"{Name}c";
    }
    
    #region Editor
    public partial class TriadMarkersLibrary
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(TriadMarkersLibrary))]
        public class TriadMarkersLibraryEditor : Editor
        {
            private const int NameFieldWidth = 100;
            private const int TextureFieldWidth = 100;
            private const int RemoveButtonWidth = 30;
            
            private TriadMarkersLibrary _origin;
            private SerializedProperty _redSpherePrefabProperty;
            private SerializedProperty _greenSpherePrefabProperty;
            private SerializedProperty _blueSpherePrefabProperty;
            private SerializedProperty _widthProperty;
            
            private void OnEnable()
            {
                _origin = target as TriadMarkersLibrary;
                _redSpherePrefabProperty = serializedObject.FindProperty(nameof(_origin._redSpherePrefab));
                _greenSpherePrefabProperty = serializedObject.FindProperty(nameof(_origin._greenSpherePrefab));
                _blueSpherePrefabProperty = serializedObject.FindProperty(nameof(_origin._blueSpherePrefab));
                _widthProperty = serializedObject.FindProperty(nameof(_origin._width));
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                DrawScriptLink();

                EditorGUILayout.PropertyField(_redSpherePrefabProperty);
                EditorGUILayout.PropertyField(_greenSpherePrefabProperty);
                EditorGUILayout.PropertyField(_blueSpherePrefabProperty);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_widthProperty);
                EditorGUILayout.Space();
                
                if (_origin._triadMarkers.Count > 0)
                {
                    DrawTableTitle();
                
                    for (int i = 0; i < _origin._triadMarkers.Count; i++)
                        DrawTriadRow(_origin._triadMarkers[i], i);
                }

                if (GUILayout.Button("Add marker"))
                    _origin._triadMarkers.Add(new TriadMarkerData());

                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                    EditorUtility.SetDirty(target);
            }
            
            private void DrawTriadRow(TriadMarkerData triadMarker, int index)
            {
                EditorGUILayout.BeginHorizontal();
                triadMarker.Name = EditorGUILayout.TextField(triadMarker.Name, GUILayout.Width(NameFieldWidth));
                triadMarker.MarkerA = EditorGUILayout.ObjectField(triadMarker.MarkerA, typeof(Texture2D), GUILayout.MinWidth(TextureFieldWidth)) as Texture2D;
                triadMarker.MarkerB = EditorGUILayout.ObjectField(triadMarker.MarkerB, typeof(Texture2D), GUILayout.MinWidth(TextureFieldWidth)) as Texture2D;
                triadMarker.MarkerC = EditorGUILayout.ObjectField(triadMarker.MarkerC, typeof(Texture2D), GUILayout.MinWidth(TextureFieldWidth)) as Texture2D;
                    
                if (GUILayout.Button("-", GUILayout.Width(RemoveButtonWidth)))
                    _origin._triadMarkers.RemoveAt(index);
                    
                EditorGUILayout.EndHorizontal();
            }

            private void DrawTableTitle()
            {
                var red = new GUIStyle(EditorStyles.label);
                red.normal.textColor = Color.red;
 
                var green = new GUIStyle(EditorStyles.label);
                green.normal.textColor = Color.green;
 
                var blue = new GUIStyle(EditorStyles.label);
                blue.normal.textColor = Color.blue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.Width(NameFieldWidth));
                EditorGUILayout.LabelField("A", red, GUILayout.MinWidth(TextureFieldWidth));
                EditorGUILayout.LabelField("B", green, GUILayout.MinWidth(TextureFieldWidth));
                EditorGUILayout.LabelField("C", blue, GUILayout.MinWidth(TextureFieldWidth));
                EditorGUILayout.Space(RemoveButtonWidth);
                EditorGUILayout.EndHorizontal();
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(_origin), typeof(TriadMarkersLibrary), false);
                GUI.enabled = true;
            }
        }
#endif
    }
    #endregion
}
