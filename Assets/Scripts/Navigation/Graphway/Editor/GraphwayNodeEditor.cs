using System;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using TargetsSystem.Points;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GraphwayNode))]
[CanEditMultipleObjects]
public class GraphwayNodeEditor : Editor
{
	private GraphwayNode origin; 
	
	private bool IsMouseOverSceneView =>  EditorWindow.mouseOverWindow.ToString() == " (UnityEditor.SceneView)";
	
	
	private void OnEnable()
	{
		origin = target as GraphwayNode;
	}

	void OnDisable()
    {
        // Hide Graph
        if (origin != null)
	        GraphwayEditor.DisableRenderers(origin.transform);
    }
	
	
	void OnGUI()
	{
		if (Event.current.type == EventType.ValidateCommand)
		{
		    switch (Event.current.commandName)
		    {
		         case "UndoRedoPerformed":
		            this.Repaint();
		            break;
		    }
		}
	}
	
	public override void OnInspectorGUI()
	{
		GraphwayEditor.CreateLogo();
		
		// Check if two nodes are selected that can be linked
		GameObject[] selectedObjects = Selection.gameObjects;

		bool allHaveNodesScript = true;

		foreach (GameObject selectedObject in selectedObjects) 
		{
			if ( ! selectedObject.GetComponent<GraphwayNode>()) 
			{
				allHaveNodesScript = false;
			}
		}

		// Check if nodes are already linked
		bool selectedObjectsLinked = false;

		if (allHaveNodesScript && selectedObjects.Length == 2)
		{
            GraphwayNode graphwayNodeA = selectedObjects[0].GetComponent<GraphwayNode>();
            GraphwayNode graphwayNodeB = selectedObjects[1].GetComponent<GraphwayNode>();

            if (NodesAreConnected(graphwayNodeA.nodeID, graphwayNodeB.nodeID) || 
                NodesAreConnected(graphwayNodeB.nodeID, graphwayNodeA.nodeID))
			{
				selectedObjectsLinked = true;
			}
		}

		// Show button to link / unlink nodes
		if (selectedObjects.Length != 2)
		{
			EditorGUILayout.HelpBox("Select two nodes to connect or disconnect them.", MessageType.Info);
		}
		
		GraphwayNode graphwayNodeData = (GraphwayNode)target;
		graphwayNodeData.nodeID = EditorGUILayout.IntField("Id", graphwayNodeData.nodeID);
		target.name = graphwayNodeData.nodeID.ToString();
		
		EditorGUI.BeginDisabledGroup(allHaveNodesScript == false || selectedObjects.Length != 2);

		if (selectedObjectsLinked)
		{
			if (GUILayout.Button("Disconnect Nodes"))
			{
				DisconnectSelectedNodes();
			}
		}
		else
		{
			if (GUILayout.Button("Connect Nodes (Bidirectional)"))
			{
				ConnectSelectedNodes(GraphwayConnectionTypes.Bidirectional);
			}
			else if (GUILayout.Button("Connect Nodes (Unidirectional A To B)"))
			{
				ConnectSelectedNodes(GraphwayConnectionTypes.UnidirectionalAToB);
			}
			else if (GUILayout.Button("Connect Nodes (Unidirectional B To A)"))
			{
				ConnectSelectedNodes(GraphwayConnectionTypes.UnidirectionalBToA);
			}
		}

		EditorGUI.EndDisabledGroup();
		
		EditorGUILayout.Space();
	}

	private void OnSceneGUI()
	{
        // Allow nodes to be linked/unlinked using the 'C' key
        // Nodes will only be linked Bidirectionally this way
		Event e = Event.current;

		switch (e.type)
		{
			case EventType.KeyDown:
			{
				if (Event.current.keyCode == KeyCode.C)
				{
					// Check if two nodes are selected that can be linked
					GameObject[] selectedObjects = Selection.gameObjects;
			
					bool allHaveNodesScript = true;
			
					foreach (GameObject selectedObject in selectedObjects) 
					{
						if ( ! selectedObject.GetComponent<GraphwayNode>()) 
						{
							allHaveNodesScript = false;
						}
					}

					// Check if nodes are already linked
					bool selectedObjectsLinked = false;
			
					if (allHaveNodesScript && selectedObjects.Length == 2)
					{
			            GraphwayNode graphwayNodeA = selectedObjects[0].GetComponent<GraphwayNode>();
			            GraphwayNode graphwayNodeB = selectedObjects[1].GetComponent<GraphwayNode>();
			
			            if (NodesAreConnected(graphwayNodeA.nodeID, graphwayNodeB.nodeID) || NodesAreConnected(graphwayNodeB.nodeID, graphwayNodeA.nodeID))
						{
							selectedObjectsLinked = true;
						}
						
						if (selectedObjectsLinked)
						{
							DisconnectSelectedNodes();
						}
						else
						{
							ConnectSelectedNodes(GraphwayConnectionTypes.Bidirectional);
						}
						
                        // Use up event
						Event.current.Use();
						
						this.Repaint();
					}
				}
			}
			break;
		}
		
		// Redraw graph
		GraphwayNode graphwayNodeData = (GraphwayNode)target;
     
		GraphwayEditor.DrawGraph(graphwayNodeData.transform);

		ProcessDrag();
	}

	private void ProcessDrag()
	{
		return;
		
		if (IsMouseOverSceneView == false)
			return;
			
		Event e = Event.current;
		GameObject pickObject = HandleUtility.PickGameObject(e.mousePosition, false);

		if (e.type == EventType.DragUpdated)
		{
			Debug.Log("drag");
			DragAndDrop.visualMode = pickObject ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Copy;
			e.Use();
		} 
		else if (e.type == EventType.DragPerform) 
		{
			Debug.Log("Drag performed");
			DragAndDrop.AcceptDrag();
			e.Use();
         
			AccessibleRoom room = pickObject? pickObject.GetComponentInParent<AccessibleRoom>() : null;
			
			if (room != null)
				room.GraphwayNode = origin;
		}
	}

	private void ConnectSelectedNodes(GraphwayConnectionTypes connectionType)
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        // Get node IDs of connected nodes
        GraphwayNode graphwayNodeA = selectedObjects[0].GetComponent<GraphwayNode>();
        GraphwayNode graphwayNodeB = selectedObjects[1].GetComponent<GraphwayNode>();

		int nodeIDA = graphwayNodeA.nodeID;
		int nodeIDB = graphwayNodeB.nodeID;
        
		GraphwayNode graphwayNodeData = (GraphwayNode)target;
		Graphway graphway = GraphwayEditor.FindGraphwayParent(graphwayNodeData.transform);
		
		origin.GraphwayConnector.ConnectSelectedNodes(graphway, nodeIDA, nodeIDB, connectionType);
    }
    
    private void DisconnectSelectedNodes()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        // Get node IDs of connected nodes
        GraphwayNode graphwayNodeA = selectedObjects[0].GetComponent<GraphwayNode>();
        GraphwayNode graphwayNodeB = selectedObjects[1].GetComponent<GraphwayNode>();

		int nodeIDA = graphwayNodeA.nodeID;
		int nodeIDB = graphwayNodeB.nodeID;
		
		GraphwayNode graphwayNodeData = (GraphwayNode)target;
		Graphway graphway = GraphwayEditor.FindGraphwayParent(graphwayNodeData.transform);
		
		origin.GraphwayConnector.DisconnectNodes(graphway, nodeIDA, nodeIDB);
    }

    private bool NodesAreConnected(int nodeIDA, int nodeIDB)
	{
		GraphwayNode graphwayNodeData = (GraphwayNode)target;
		Graphway graphway = GraphwayEditor.FindGraphwayParent(graphwayNodeData.transform);

		return origin.GraphwayConnector.NodesAreConnected(graphway, nodeIDA, nodeIDB);
	}
}