using UnityEditor;
using UnityEngine;

namespace Map.Signs
{
    [CreateAssetMenu(menuName = "SignPreset", fileName = "SignPreset", order = 51)]
    public partial class SignPreset : ScriptableObject
    {
        [SerializeField] private bool _hasName;
        [SerializeField] private bool _hasIcon;
        [SerializeField] private PointType pointType;

        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;

        public bool HasName => _hasName;
        public bool HasIcon => _hasIcon;
        public PointType PointType => pointType;
        public string Name => _name;
        public Sprite Icon => _icon;
    }
    
    public enum PointType
    {
        None,
        ManToilet,
        WomanToilet,
        Stairs,
        Elevator,
        ATM,
        Print,
        Library,
        Buffet,
        Marker
    }

    #region Editor
    public partial class SignPreset
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(SignPreset))]
        public class SignPresetEditor : Editor
        {
            private SignPreset _origin;
            
            private void OnEnable()
            {
                _origin = target as SignPreset;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                int interval = 20;

                var hasNameProperty = serializedObject.FindProperty(nameof(_origin._hasName));
                EditorGUILayout.PropertyField(hasNameProperty, new GUIContent("Has name"));
                //_origin._hasName = EditorGUILayout.Toggle("Has name", _origin._hasName);

                if (_origin._hasName)
                {
                    _origin._name = EditorGUILayout.TextField("Name", _origin._name);
                    EditorGUILayout.Space(interval);
                }
                
                
                _origin._hasIcon = EditorGUILayout.Toggle("Has icon", _origin._hasIcon);

                if (_origin._hasIcon)
                {
                    _origin._icon = EditorGUILayout.ObjectField("Icon", _origin._icon, typeof(Sprite)) as Sprite;
                    EditorGUILayout.Space(interval);
                }

                _origin.pointType = (PointType)EditorGUILayout.EnumPopup("Point type", _origin.pointType);
                EditorGUILayout.Space(interval);

                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                    EditorUtility.SetDirty(target);
            }
        }
#endif
    }
    #endregion
}
