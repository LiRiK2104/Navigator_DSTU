using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GraphwayConnector : MonoBehaviour
{
    private const string ConnectionsParentName = "Connections";
    
    
    public void ConnectSelectedNodes(Graphway graphway, int nodeIDA, int nodeIDB, GraphwayConnectionTypes connectionType)
    {
        // NOTE - Nodes are connected by smallest node ID to largest node ID
        // Create connection
        if (nodeIDA < nodeIDB)
        {
            CreateConnectedNodeObj(graphway, nodeIDA, nodeIDB, connectionType);
        }
        else {
            CreateConnectedNodeObj(graphway, nodeIDB, nodeIDA, connectionType);
        }

        // Mark scene as dirty to trigger 'Save Changes' prompt
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    public void DisconnectNodes(Graphway graphway, int nodeIDA, int nodeIDB)
    {
        // NOTE - Nodes are connected by smallest node ID to largest node ID
        // Break connection
        if (nodeIDA < nodeIDB)
        {
            RemoveConnectedNodeObj(graphway, nodeIDA, nodeIDB);
        }
        else {
            RemoveConnectedNodeObj(graphway, nodeIDB, nodeIDA);
        }

        // Mark scene as dirty to trigger 'Save Changes' prompt
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    public bool NodesAreConnected(Graphway graphway, int nodeIDA, int nodeIDB)
    {
        return graphway.transform.Find(GetConnectionPath(nodeIDA, nodeIDB));
    }
    
    private void CreateConnectedNodeObj(Graphway graphway, int nodeIDA, int nodeIDB, GraphwayConnectionTypes connectionType)
    {
        if (NodesAreConnected(graphway, nodeIDA, nodeIDB) == false)
        {
            GameObject newConnection = new GameObject();

            newConnection.name = GetConnectionName(nodeIDA, nodeIDB);
            newConnection.transform.parent = GetConnectionParent(graphway);
            newConnection.AddComponent<GraphwayConnection>().SetConnectionData(nodeIDA, nodeIDB, connectionType);
            
            // Register undo operation
            Undo.RegisterCreatedObjectUndo(newConnection, "Graphway Connection");
			
            // Reorder connection hierarchy to keep things tidy
            ReorderConnections(graphway);
        }
    }

    private void RemoveConnectedNodeObj(Graphway graphway, int nodeIDA, int nodeIDB)
    {
        if (NodesAreConnected(graphway, nodeIDA, nodeIDB))
        {   
            DestroyImmediate(graphway.transform.Find(GetConnectionPath(nodeIDA, nodeIDB)).gameObject);
        }
    }

    private void ReorderConnections(Graphway graphway)
    {
        Transform connections = GetConnectionParent(graphway);
		
        // Create list of connections
        List<string> connectionNames = new List<string>();
		
        foreach (Transform connection in connections)
        {
            connectionNames.Add(connection.name);
        }
		
        // Sort list
        connectionNames.Sort();
		
        // Reorder gameobjects
        for (int i = 0 ; i < connectionNames.Count ; i++)
        {
            string connectionName = connectionNames[i];
			
            connections.Find(connectionName).SetSiblingIndex(i);
        }
    }

    private Transform GetConnectionParent(Graphway graphway)
    {
        return graphway.transform.Find(ConnectionsParentName).transform;
    }

    private string GetConnectionPath(int nodeIDA, int nodeIDB)
    {
        return $"{ConnectionsParentName}/{GetConnectionName(nodeIDA, nodeIDB)}";
    }

    private string GetConnectionName(int nodeIDA, int nodeIDB)
    {
        return $"{nodeIDA}->{nodeIDB}";
    }
}
