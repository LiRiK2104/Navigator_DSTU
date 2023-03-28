using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UI.StateSystem.Groups;
using UI.StateSystem.States;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UI.StateSystem
{
    public partial class UIStatesStorage : MonoBehaviour
    {
        [FormerlySerializedAs("_states")] 
        [SerializeField] private List<StateContainer> _statesContainers = new List<StateContainer>();
        [SerializeField] private List<Widget> _widgetsDefault = new List<Widget>();
        [SerializeField] private List<StatesGroup> _statesGroups = new List<StatesGroup>();

        public event Action<int> StateRemoved;

        public ReadOnlyCollection<StatesGroup> StatesGroups => _statesGroups.AsReadOnly();

        public bool TryGetState(StateType stateType, out StateContainer foundStateContainer)
        {
            foundStateContainer = null;

            foreach (var stateContainer in _statesContainers)
            {
                if (stateContainer.Type == stateType)
                {
                    foundStateContainer = stateContainer;
                    return true;
                }
            }

            return false;
        }


        private void AddState()
        {
            _statesContainers.Add(new StateContainer(StateType.Default, _widgetsDefault));
        }
        
        private void RemoveState(StateContainer stateContainer)
        {
            if (TryGetStateIndex(stateContainer, out int removedStateIndex) == false)
            {
                Debug.LogError("RemovedState not found!");
                return;
            }
            
            _statesContainers.Remove(stateContainer);
            StateRemoved?.Invoke(removedStateIndex);
        }
        
        private void AddWidget()
        {
            var widget = new Widget(null, false);
            _widgetsDefault.Add(widget);

            foreach (var state in _statesContainers)
                state.Widgets.Add(widget);
        }
        
        private void RemoveWidget(int widgetIndex)
        {
            _widgetsDefault.RemoveAt(widgetIndex);

            foreach (var state in _statesContainers)
                state.Widgets.RemoveAt(widgetIndex);
        }

        private bool TryGetStateIndex(StateContainer foundStateContainer, out int index)
        {
            index = 0;
            
            for (int i = 0; i < _statesContainers.Count; i++)
            {
                if (foundStateContainer == _statesContainers[i])
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
    }

    public enum StateType
    {
        MapView,
        Default,
        SearchPanel,
        SearchResults,
        PointInfo
    }

    [Serializable]
    public class StateContainer
    {
        [SerializeField] public StateType Type;
        [SerializeField] public State State;
        [SerializeField] public List<Widget> Widgets = new List<Widget>();

        public StateContainer(StateType type, List<Widget> widgets)
        {
            Type = type;
            Widgets = widgets.ToList();
            State = null;
        }
    }
    
    [Serializable]
    public struct Widget
    {
        [SerializeField] public GameObject GameObject;
        [SerializeField] public bool Active;

        public Widget(GameObject gameObject, bool active)
        {
            GameObject = gameObject;
            Active = active;
        }
    }

    #region Editor
    public partial class UIStatesStorage
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(UIStatesStorage))]
        public class UIStatesStorageEditor : Editor
        {
            private const int StateFieldWidth = 30;
            private const int DefaultIndentedLevel = 15;
            private const int DefaultInterval = 2;
            
            private UIStatesStorage _origin;
            

            private void OnEnable()
            {
                _origin = target as UIStatesStorage;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                DrawScriptLink();
                
                int widgetRectWidth = 120;
                int widgetFieldHeight = 20;
                int lastRectMaxHeight = 18;
                
                DrawWidgets(widgetRectWidth, widgetFieldHeight, lastRectMaxHeight);
                DrawStates(widgetRectWidth, widgetFieldHeight);
                DrawRemoveWidgetButtons(widgetRectWidth);
                DrawControlButtons();
                DrawGroups();

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawWidgets(int widgetRectWidth, int widgetFieldHeight, float lastRectBottom)
            {
                int rotateAngle = 90;
                Vector2 pivotPoint = new Vector2(
                    DefaultIndentedLevel + widgetRectWidth, 
                    DefaultIndentedLevel + widgetRectWidth + lastRectBottom);
                
                int widgetRectHeight = (widgetFieldHeight + DefaultInterval) * _origin._widgetsDefault.Count;
                
                
                GUIUtility.RotateAroundPivot(rotateAngle, pivotPoint);

                float rectY = DefaultIndentedLevel + widgetRectWidth - widgetRectHeight + lastRectBottom;
                GUILayout.BeginArea(new Rect(DefaultIndentedLevel, rectY, widgetRectWidth, widgetRectHeight));

                var widgetFieldLayoutOption = new [] {GUILayout.Width(widgetRectWidth), GUILayout.Height(widgetFieldHeight)};

                for (int i = 0; i < _origin._widgetsDefault.Count; i++)
                {
                    var widgetTemplate = _origin._widgetsDefault[i];
                    widgetTemplate.GameObject = EditorGUILayout.ObjectField(widgetTemplate.GameObject, typeof(GameObject), true, widgetFieldLayoutOption) as GameObject;
                    _origin._widgetsDefault[i] = widgetTemplate;

                    foreach (var state in _origin._statesContainers)
                        state.Widgets[i] = new Widget(widgetTemplate.GameObject, state.Widgets[i].Active);
                }


                GUILayout.EndArea();
                GUIUtility.RotateAroundPivot(-rotateAngle, pivotPoint);
                GUILayout.Space(widgetRectWidth + DefaultIndentedLevel);
            }

            private void DrawStates(int widgetRectWidth, int widgetFieldHeight)
            {
                for (int i = 0; i < _origin._statesContainers.Count; i++)
                    DrawStateAndToggles(_origin._statesContainers[i], widgetRectWidth, widgetFieldHeight);
            }

            private void DrawStateAndToggles(StateContainer stateContainer, int widgetRectWidth, int widgetFieldHeight)
            {
                int toggleWidth = 19;
                var toggleLayoutOption = new [] {GUILayout.Width(toggleWidth), GUILayout.Height(widgetFieldHeight)};
                
                GUILayout.BeginHorizontal();
                stateContainer.Type = (StateType)EditorGUILayout.EnumPopup(stateContainer.Type, GUILayout.Width(widgetRectWidth / 2));
                stateContainer.State = EditorGUILayout.ObjectField(stateContainer.State, typeof(State), true, GUILayout.Width(widgetRectWidth / 2)) as State;

                for (int i = stateContainer.Widgets.Count - 1; i >= 0; i--)
                {
                    var widget = stateContainer.Widgets[i];
                    widget.Active = GUILayout.Toggle(widget.Active, String.Empty, toggleLayoutOption);
                    stateContainer.Widgets[i] = widget;
                }
                
                DrawRemoveStateButton(stateContainer);
                GUILayout.EndHorizontal();
            }

            private void DrawRemoveWidgetButtons(int widgetRectWidth)
            {
                int buttonWidth = 19;
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(widgetRectWidth);

                for (int i = _origin._widgetsDefault.Count - 1; i >= 0; i--)
                {
                    if (GUILayout.Button("-", GUILayout.Width(buttonWidth)))
                        _origin.RemoveWidget(i);
                }
                
                GUILayout.EndHorizontal();
            }

            private void DrawRemoveStateButton(StateContainer stateContainer)
            {
                int width = 30;
                
                if (GUILayout.Button("-", GUILayout.Width(width)))
                    _origin.RemoveState(stateContainer);
            }

            private void DrawControlButtons()
            {
                int space = 30;
                
                GUILayout.Space(space);
                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Add State"))
                    _origin.AddState();
                
                GUILayout.Space(space);
                
                if (GUILayout.Button("Add Widget"))
                    _origin.AddWidget();
                
                GUILayout.EndHorizontal();
                GUILayout.Space(space);
            }

            private void DrawGroups()
            {
                var groupsProperty = serializedObject.FindProperty(nameof(_origin._statesGroups));
                EditorGUILayout.PropertyField(groupsProperty, new GUIContent("Groups"));
            }
            
            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(UIStatesStorage), false);
                GUI.enabled = true;
            }
        }
#endif
    }
    #endregion
}
