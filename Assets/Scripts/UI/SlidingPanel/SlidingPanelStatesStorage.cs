using System;
using System.Collections.Generic;
using System.Linq;
using UI.StateSystem;
using UnityEditor;
using UnityEngine;

namespace UI.SlidingPanel
{
    [RequireComponent(typeof(SlidingPanelHandler))]
    public partial class SlidingPanelStatesStorage : MonoBehaviour
    {
        [SerializeField] private List<SearchPanelStatePreset> _statePresets;

        private SlidingPanelHandler _slidingPanelHandler;
        
        private SlidingPanelHandler SlidingPanelHandler 
        {
            get
            {
                if (_slidingPanelHandler == null)
                    _slidingPanelHandler = GetComponent<SlidingPanelHandler>();

                return _slidingPanelHandler;
            }
        }
        
        private UIStatesStorage UIStatesStorage => Global.Instance.UISetterV2.UIStatesStorage;

        
        protected void OnEnable()
        {
            UIStatesStorage.StateRemoved += UpdateIndex;
        }

        protected void OnDisable()
        {
            UIStatesStorage.StateRemoved -= UpdateIndex;
        }


        public bool TryGetState(Transform point, out StateType stateType)
        {
            stateType = StateType.Default;
            
            foreach (var statePreset in _statePresets)
            {
                if (statePreset.TargetPoint == point)
                {
                    stateType = statePreset.StateType;
                    return true;
                }
            }

            return false;
        }

        private void UpdateIndex(int removedStateIndex)
        {
            for (int i = 0; i < _statePresets.Count; i++)
            {
                var updatedPreset = _statePresets[i];
                
                if ((int)_statePresets[i].StateType > removedStateIndex)
                    updatedPreset.StateType--;
                else if ((int)_statePresets[i].StateType == removedStateIndex)
                    updatedPreset.StateType = 0;

                _statePresets[i] = updatedPreset;
            }
        }

        private void UpdateStatePresets()
        {
            var statePresetsToDelete = new List<SearchPanelStatePreset>();

            foreach (var statePreset in _statePresets)
            {
                if (SlidingPanelHandler.TargetPoints.Contains(statePreset.TargetPoint) == false)
                    statePresetsToDelete.Add(statePreset);
            }

            foreach (var presetToDelete in statePresetsToDelete)
                _statePresets.Remove(presetToDelete);
            
            foreach (var targetPoint in SlidingPanelHandler.TargetPoints)
            {
                if (_statePresets.Select(statePreset => statePreset.TargetPoint).Contains(targetPoint) == false)
                    _statePresets.Add(new SearchPanelStatePreset(targetPoint));
            }
        }
    }
    
    [Serializable]
    public struct SearchPanelStatePreset
    {
        private const StateType DefaultStateType = StateType.Default; 
        
        public Transform TargetPoint;
        public StateType StateType;

        public SearchPanelStatePreset(Transform targetPoint)
        {
            TargetPoint = targetPoint;
            StateType = DefaultStateType;
        }
        
        public SearchPanelStatePreset(Transform targetPoint, StateType stateType)
        {
            TargetPoint = targetPoint;
            StateType = stateType;
        }
    }

    #region Editor
    public partial class SlidingPanelStatesStorage
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(SlidingPanelStatesStorage))]
        public class SlidingPanelStateSetterEditor : Editor
        {
            private SlidingPanelStatesStorage _origin;
            
            private void OnEnable()
            {
                _origin = target as SlidingPanelStatesStorage;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                DrawScriptLink();
                _origin.UpdateStatePresets();
                    
                for (int i = 0; i < _origin._statePresets.Count; i++)
                {
                    var statePreset = _origin._statePresets[i];

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(statePreset.TargetPoint.gameObject.name);
                    StateType stateType = (StateType)EditorGUILayout.EnumPopup(statePreset.StateType);
                    GUILayout.EndHorizontal();

                    _origin._statePresets[i] = new SearchPanelStatePreset(statePreset.TargetPoint, stateType);
                }

                serializedObject.ApplyModifiedProperties();
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(SlidingPanelStatesStorage), false);
                GUI.enabled = true;
            }
        }
#endif
    }
    #endregion
}
