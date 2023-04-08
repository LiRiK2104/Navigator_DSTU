using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Plugins.ZenythStudios.Graphway.Assets.Scripts
{
    [RequireComponent(typeof(Graphway))]
    public partial class GraphwayRenamer : MonoBehaviour
    {
        private const int MinFloor = 1;
        private const int MaxFloor = 4;
        
        [SerializeField] private Graphway _graphway;
        [SerializeField] private int _floor;
    }

    #region Editor

    public partial class GraphwayRenamer
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(GraphwayRenamer))]
        public class GraphwayRenamerEditor : Editor
        {
            private GraphwayRenamer _origin;

            private void OnEnable()
            {
                _origin = target as GraphwayRenamer;
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DrawScriptLink();
                
                _origin._graphway = EditorGUILayout.ObjectField(_origin._graphway, typeof(Graphway)) as Graphway;
                _origin._floor = EditorGUILayout.IntSlider(_origin._floor, MinFloor, MaxFloor);
                var oldNewIds = new Dictionary<int, int>();

                if (_origin._graphway != null && 
                    GUILayout.Button("Generate nodes IDs"))
                {
                    int number = 1000 * _origin._floor;
                    var nodes = _origin._graphway.GetAllNodes();
                    
                    foreach (var node in nodes)
                    {
                        oldNewIds[node.nodeID] = number;
                        node.SetNodeID(number++);
                    }

                    var connections = _origin._graphway.GetAllConnections();
                    
                    foreach (var connection in connections)
                    {
                        if (oldNewIds.ContainsKey(connection.nodeIDA) && 
                            oldNewIds.ContainsKey(connection.nodeIDB))
                        {
                            connection.nodeIDA = oldNewIds[connection.nodeIDA];
                            connection.nodeIDB = oldNewIds[connection.nodeIDB];
                            connection.UpdateName();
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            private void UpdateIds()
            {
                int number = 1000 * _origin._floor;
                var nodes = _origin._graphway.GetAllNodes();
                    
                foreach (var node in nodes)
                    node.SetNodeID(number++);
            }

            private void DrawScriptLink()
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(_origin), typeof(GraphwayRenamer), false);
                GUI.enabled = true;
            }
        }
#endif
    }

    #endregion
}
