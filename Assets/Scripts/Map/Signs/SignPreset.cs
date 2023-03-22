using System;
using UnityEditor;
using UnityEngine;

namespace Map.Signs
{
    [CreateAssetMenu(menuName = "SignPreset", fileName = "SignPreset", order = 51)]
    public partial class SignPreset : ScriptableObject
    {
        [SerializeField] private bool _hasName;
        [SerializeField] private bool _hasIcon;
        [SerializeField] private bool _isTransitPoint;
        [SerializeField] private TransitType _transitType;

        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;

        public bool HasName => _hasName;
        public bool HasIcon => _hasIcon;
        public bool IsTransitPoint => _isTransitPoint;
        public TransitType TransitType => _transitType;
        public string Name => _name;
        public Sprite Icon => _icon;
    }

    public enum TransitType
    {
        Stairs,
        Elevator
    }
    
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
                
                _origin._hasName = EditorGUILayout.Toggle("Has name", _origin._hasName);

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
                
                
                _origin._isTransitPoint = EditorGUILayout.Toggle("Is transit point", _origin._isTransitPoint);
                
                if (_origin._isTransitPoint)
                {
                    _origin._transitType = (TransitType)EditorGUILayout.EnumPopup("Transit type", _origin._transitType);
                    EditorGUILayout.Space(interval);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
