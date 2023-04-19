using UnityEngine;
using UnityEditor;
using System.Linq;
using UI.StateSystem;
using UI.StateSystem.Setters;

namespace UI.SlidingPanel.Setters
{
    public partial class SlidingPanelSwitchObject : ExternalStateSetter
    {
        [SerializeField] private SlidingPanelHandler _slidingPanelHandler;
        [SerializeField] protected int _index;
        [SerializeField] private bool _instantlySet;

        protected void SetPanelPosition()
        {
            _slidingPanelHandler.SwitchPosition(_index, SetState, _instantlySet);
        }
        
        private void SetState(Transform targetPoint)
        {
            if (_slidingPanelHandler.StatesStorage.TryGetState(targetPoint, out StateType stateType))
                SetState(stateType);
        }
    }
    
    #region Editor
    public partial class SlidingPanelSwitchObject
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(SlidingPanelSwitchObject), true)]
        public class SlidingPanelSwitchObjectEditor : Editor
        {
            private SlidingPanelSwitchObject _origin;
            
            private void OnEnable()
            {
                _origin = target as SlidingPanelSwitchObject;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                DrawScriptLink();
                
                _origin._slidingPanelHandler = EditorGUILayout.ObjectField("Sliding Panel Handler", _origin._slidingPanelHandler, typeof(SlidingPanelHandler)) as SlidingPanelHandler;

                if (_origin._slidingPanelHandler != null)
                {
                    _origin._index = 
                        EditorGUILayout.Popup("Target Point", _origin._index, 
                            _origin._slidingPanelHandler.TargetPoints.Select(targetPoint => targetPoint.gameObject.name).ToArray(), 
                            EditorStyles.popup);
                    
                    _origin._instantlySet = EditorGUILayout.Toggle("Set instantly", _origin._instantlySet);
                }
                
                serializedObject.ApplyModifiedProperties();
                
                if (GUI.changed)
                    EditorUtility.SetDirty(_origin);
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(SlidingPanelSwitchObject), false);
                GUI.enabled = true;
            }
        }
#endif
    }
    #endregion
}
