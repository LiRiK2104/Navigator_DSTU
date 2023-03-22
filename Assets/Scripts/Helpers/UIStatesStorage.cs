using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Helpers
{
    public partial class UIStatesStorage : MonoBehaviour
    {
        [SerializeField] private List<UIState> _states = new List<UIState>();
        [SerializeField] private List<Widget> _widgetsDefault = new List<Widget>();
        
        public event Action<int> StateRemoved;


        public bool TryGetState(int index, out UIState foundState)
        {
            foundState = null;
            
            if (index >= 0 && index < _states.Count)
            {
                foundState = _states[index];
                return true;
            }

            return false;
        }
        
        public string[] GetStatesNames()
        {
            return _states.Select(state => state.Name).ToArray();
        }
        
        private void AddState()
        {
            _states.Add(new UIState("State", _widgetsDefault));
        }
        
        private void RemoveState(UIState state)
        {
            if (TryGetStateIndex(state, out int removedStateIndex) == false)
            {
                Debug.LogError("RemovedState not found!");
                return;
            }
            
            _states.Remove(state);
            StateRemoved?.Invoke(removedStateIndex);
        }
        
        private void AddWidget()
        {
            var widget = new Widget(null, false);
            _widgetsDefault.Add(widget);

            foreach (var state in _states)
                state.Widgets.Add(widget);
        }
        
        private void RemoveWidget(int widgetIndex)
        {
            _widgetsDefault.RemoveAt(widgetIndex);

            foreach (var state in _states)
                state.Widgets.RemoveAt(widgetIndex);
        }

        private bool TryGetStateIndex(UIState foundState, out int index)
        {
            index = 0;
            
            for (int i = 0; i < _states.Count; i++)
            {
                if (foundState == _states[i])
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public class UIState
    {
        [SerializeField] public string Name;
        [SerializeField] public List<Widget> Widgets = new List<Widget>();
        [SerializeField] public ExampleEvent OnEvent = new ExampleEvent();

        public UIState(string name, List<Widget> widgets)
        {
            Name = name;
            Widgets = widgets.ToList();
        }
        
        [Serializable]
        public class ExampleEvent : UnityEvent {}
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

    public partial class UIStatesStorage
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(UIStatesStorage))]
        public class UIStatesStorageEditor : Editor
        {
            private const int DefaultIndentedLevel = 15;
            private const int DefaultInterval = 2;
            
            private UIStatesStorage _origin;
            

            private void OnEnable()
            {
                _origin = target as UIStatesStorage;
            }

            public override void OnInspectorGUI()
            {
                int widgetRectWidth = 100;
                int widgetFieldHeight = 20;

                Vector2 statesRectSize = EditorStyles.label.CalcSize(new GUIContent(_origin._states
                    .OrderBy(state => state.Name.Length).Select(state => state.Name).FirstOrDefault()));
                statesRectSize.y *= _origin._states.Count;

                DrawWidgets(widgetRectWidth, widgetFieldHeight);
                DrawStates(widgetRectWidth, widgetFieldHeight);
                DrawRemoveWidgetButtons(widgetRectWidth);
                DrawControlButtons();
                DrawEvents();

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawWidgets(int widgetRectWidth, int widgetFieldHeight)
            {
                int rotateAngle = 90;
                Vector2 pivotPoint = new Vector2(
                    DefaultIndentedLevel + widgetRectWidth, 
                    DefaultIndentedLevel + widgetRectWidth);
                
                int widgetRectHeight = (widgetFieldHeight + DefaultInterval) * _origin._widgetsDefault.Count;
                
                
                GUIUtility.RotateAroundPivot(rotateAngle, pivotPoint);

                int rectY = DefaultIndentedLevel + widgetRectWidth - widgetRectHeight;
                GUILayout.BeginArea(new Rect(DefaultIndentedLevel, rectY, widgetRectWidth, widgetRectHeight));

                var widgetFieldLayoutOption = new [] {GUILayout.Width(widgetRectWidth), GUILayout.Height(widgetFieldHeight)};

                for (int i = 0; i < _origin._widgetsDefault.Count; i++)
                {
                    var widgetTemplate = _origin._widgetsDefault[i];
                    widgetTemplate.GameObject = EditorGUILayout.ObjectField(widgetTemplate.GameObject, typeof(GameObject), true, widgetFieldLayoutOption) as GameObject;
                    _origin._widgetsDefault[i] = widgetTemplate;

                    foreach (var state in _origin._states)
                        state.Widgets[i] = new Widget(widgetTemplate.GameObject, state.Widgets[i].Active);
                }


                GUILayout.EndArea();
                GUIUtility.RotateAroundPivot(-rotateAngle, pivotPoint);
                GUILayout.Space(widgetRectWidth + DefaultIndentedLevel);
            }

            private void DrawStates(int widgetRectWidth, int widgetFieldHeight)
            {
                for (int i = 0; i < _origin._states.Count; i++)
                    DrawStateAndToggles(_origin._states[i], widgetRectWidth, widgetFieldHeight);
            }

            private void DrawStateAndToggles(UIState uiState, int widgetRectWidth, int widgetFieldHeight)
            {
                int toggleWidth = 19;
                var toggleLayoutOption = new [] {GUILayout.Width(toggleWidth), GUILayout.Height(widgetFieldHeight)};
                
                GUILayout.BeginHorizontal();
                uiState.Name = GUILayout.TextField(uiState.Name, GUILayout.Width(widgetRectWidth));

                for (int i = uiState.Widgets.Count - 1; i >= 0; i--)
                {
                    var widget = uiState.Widgets[i];
                    widget.Active = GUILayout.Toggle(widget.Active, String.Empty, toggleLayoutOption);
                    uiState.Widgets[i] = widget;
                }
                
                DrawRemoveStateButton(uiState);
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

            private void DrawRemoveStateButton(UIState uiState)
            {
                int width = 30;
                
                if (GUILayout.Button("-", GUILayout.Width(width)))
                    _origin.RemoveState(uiState);
            }

            private void DrawEvents()
            {
                var statesProperty = serializedObject.FindProperty(nameof(_origin._states));

                for (int i = 0; i < _origin._states.Count; i++)
                {
                    var stateProperty = statesProperty.GetArrayElementAtIndex(i);
                    var state = _origin._states[i];
                    var eventProperty = stateProperty.FindPropertyRelative(nameof(state.OnEvent));
                    
                    serializedObject.Update();
                    EditorGUILayout.PropertyField(eventProperty, new GUIContent($"{state.Name} set event"), true);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
            private void DrawEvent(UIState state)
            {
                serializedObject.Update();
                var serializedProperty = serializedObject.FindProperty(nameof(state.OnEvent));
                Debug.Log($"\"{nameof(state.OnEvent)}\"");
                //EditorGUILayout.PropertyField(serializedProperty, true);
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
        }
#endif
    }
}
