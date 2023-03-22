using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEditor;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(SlidingPanelHandler))]
    public partial class SlidingPanelStateSetter : StateSetter
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

        
        protected override void OnEnable()
        {
            base.OnEnable();
            SlidingPanelHandler.PositionChanged += SetState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UIStatesStorage.StateRemoved -= UpdateIndex;
        }
        

        private void SetState(Transform targetPoint)
        {
            foreach (var statePreset in _statePresets)
            {
                if (statePreset.TargetPoint == targetPoint)
                    SetState(statePreset.UIStateIndex);
            }
        }
        
        protected override void UpdateIndex(int removedStateIndex)
        {
            for (int i = 0; i < _statePresets.Count; i++)
            {
                var updatedPreset = _statePresets[i];
                
                if (_statePresets[i].UIStateIndex > removedStateIndex)
                    updatedPreset.UIStateIndex--;
                else if (_statePresets[i].UIStateIndex == removedStateIndex)
                    updatedPreset.UIStateIndex = 0;

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

    public partial class SlidingPanelStateSetter
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(SlidingPanelStateSetter))]
        public class SlidingPanelStateSetterEditor : Editor
        {
            private SlidingPanelStateSetter _origin;
            
            private void OnEnable()
            {
                _origin = target as SlidingPanelStateSetter;
            }

            public override void OnInspectorGUI()
            {
                DrawScriptLink();
                _origin.UIStatesStorage = EditorGUILayout.ObjectField("UI States Storage", _origin.UIStatesStorage, typeof(UIStatesStorage)) as UIStatesStorage;
                
                if (_origin.UIStatesStorage != null)
                {
                    _origin.UpdateStatePresets();
                    
                    for (int i = 0; i < _origin._statePresets.Count; i++)
                    {
                        var statePreset = _origin._statePresets[i];

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(statePreset.TargetPoint.gameObject.name);
                        int index = EditorGUILayout.Popup(statePreset.UIStateIndex,
                            _origin.UIStatesStorage.GetStatesNames(), EditorStyles.popup);
                        GUILayout.EndHorizontal();

                        _origin._statePresets[i] = new SearchPanelStatePreset(statePreset.TargetPoint, index);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(SlidingPanelStateSetter), false);
                GUI.enabled = true;
            }
        }
#endif
    }

    [Serializable]
    public struct SearchPanelStatePreset
    {
        private const int DefaultIndex = 0; 
        
        public Transform TargetPoint;
        public int UIStateIndex;

        public SearchPanelStatePreset(Transform targetPoint)
        {
            TargetPoint = targetPoint;
            UIStateIndex = DefaultIndex;
        }
        
        public SearchPanelStatePreset(Transform targetPoint, int uiStateIndex)
        {
            TargetPoint = targetPoint;
            UIStateIndex = uiStateIndex;
        }
    }
}
