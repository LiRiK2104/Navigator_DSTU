using System;
using Map.Signs;
using Navigation;
using UnityEditor;
using UnityEngine;

namespace TargetsSystem.Points
{
    public abstract partial class Point : MonoBehaviour
    {
        [SerializeField] private bool _isWayPoint = true;
        [SerializeField] private GraphwayNode _graphwayNode;
        [SerializeField] private SignCreator _signCreator;

        public bool IsWayPoint => _isWayPoint;
        public SignCreator SignCreator => _signCreator;
        public GraphwayNode GraphwayNode
        {
            get => _graphwayNode;
            set => _graphwayNode = value;
        }
        public Vector3 GraphwayNodePosition => _graphwayNode.transform.position;
        private DataBase DataBase => Global.Instance.DataBase;
        
        
        public virtual void Initialize()
        {
            if (TryGetInfo(out PointInfo pointInfo))
                _signCreator.Create(pointInfo);
        }

        public bool TryGetInfo(out PointInfo pointInfo)
        {
            return DataBase.TryGetPointInfo(this, out pointInfo);
        }
        
        private void OnDrawGizmos()
        {
            if (_graphwayNode != null)
            {
                var sphereRadius = 0.4f;
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, GraphwayNodePosition);
                Gizmos.DrawSphere(GraphwayNodePosition, sphereRadius);
            }
        }
    }

    #region Editor
    public abstract partial class Point
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(Point), true)]
        [CanEditMultipleObjects]
        public class PointEditor : Editor
        {
            private Point _origin;
            private SerializedProperty _isWayPointProperty;
            private SerializedProperty _graphwayNodeProperty;
            private SerializedProperty _signCreatorProperty;

            protected void OnEnable()
            {
                _origin = target as Point;
                _isWayPointProperty = serializedObject.FindProperty(nameof(_origin._isWayPoint));
                _graphwayNodeProperty = serializedObject.FindProperty(nameof(_origin._graphwayNode));
                _signCreatorProperty = serializedObject.FindProperty(nameof(_origin._signCreator));
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(_isWayPointProperty);

                if (_origin._isWayPoint)
                {
                    EditorGUILayout.PropertyField(_graphwayNodeProperty);
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.PropertyField(_signCreatorProperty);

                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                    EditorUtility.SetDirty(target);
            }
        }
#endif
    }
    #endregion
}
