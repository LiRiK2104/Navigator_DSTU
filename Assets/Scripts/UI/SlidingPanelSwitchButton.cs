using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public partial class SlidingPanelSwitchButton : MonoBehaviour
    {
        [SerializeField] private SlidingPanelHandler _slidingPanelHandler;
        [SerializeField] private int _index;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => _slidingPanelHandler.SwitchPosition(_index));
        }
    }
    
    public partial class SlidingPanelSwitchButton
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(SlidingPanelSwitchButton))]
        public class SlidingPanelSwitchButtonEditor : Editor
        {
            private SlidingPanelSwitchButton _origin;
            
            private void OnEnable()
            {
                _origin = target as SlidingPanelSwitchButton;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                _origin._slidingPanelHandler = EditorGUILayout.ObjectField("Sliding Panel Handler", _origin._slidingPanelHandler, typeof(SlidingPanelHandler)) as SlidingPanelHandler;

                if (_origin._slidingPanelHandler != null)
                {
                    _origin._index = 
                        EditorGUILayout.Popup("Target Point", _origin._index, 
                            _origin._slidingPanelHandler.TargetPoints.Select(targetPoint => targetPoint.gameObject.name).ToArray(), 
                            EditorStyles.popup);   
                }
                
                serializedObject.ApplyModifiedProperties();
                
                if (GUI.changed)
                    EditorUtility.SetDirty(_origin);
            }
        }
#endif
    }
}
