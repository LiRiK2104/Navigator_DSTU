using Map.Signs;
using UnityEditor;
using UnityEngine;

namespace TargetsSystem.Points
{
    public partial class AccessibleRoom : Point, IRoom
    {
        [SerializeField] private string _id;
        [SerializeField] private Color _fieldColor = Color.white;
        [SerializeField] private SpriteRenderer _field;

        public string Id => _id;


        public override void Initialize()
        {
            base.Initialize();
            PaintField();
        }
        
        public void PaintField()
        {
            _field.color = _fieldColor;
        }
    }

    public interface IRoom
    {
        public string Id { get; }

        public void PaintField();
    }

    #region Editor
    public partial class AccessibleRoom
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(AccessibleRoom))]
        [CanEditMultipleObjects]
        public class AccessibleRoomEditor : PointEditor
        {
            private AccessibleRoom _accessibleRoomOrigin;
            private SerializedProperty _idProperty;
            private SerializedProperty _fieldColorProperty;
            private SerializedProperty _fieldProperty;

            private new void OnEnable()
            {
                base.OnEnable();
                _accessibleRoomOrigin = target as AccessibleRoom;
                _idProperty = serializedObject.FindProperty(nameof(_accessibleRoomOrigin._id));
                _fieldColorProperty = serializedObject.FindProperty(nameof(_accessibleRoomOrigin._fieldColor));
                _fieldProperty = serializedObject.FindProperty(nameof(_accessibleRoomOrigin._field));
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();

                EditorGUILayout.PropertyField(_idProperty);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_fieldColorProperty);
                EditorGUILayout.PropertyField(_fieldProperty);

                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                    EditorUtility.SetDirty(target);
            }
        }
#endif
    }
    #endregion
}
