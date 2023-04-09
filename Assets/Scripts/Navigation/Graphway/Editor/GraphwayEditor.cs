using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins.ZenythStudios.Graphway.Assets.Scripts;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(Graphway))]
public class GraphwayEditor : Editor 
{
	const int NODE_FONT_SIZE = 20;
	const int SUBNODE_FONT_SIZE = 18;
	
	private SerializedProperty nodeColor;
	private SerializedProperty subnodeColor;
	private SerializedProperty nodeSize;
	private SerializedProperty subnodeSize;
	private SerializedProperty arrowSize;
	private SerializedProperty pathfindFrameLimit;
	private SerializedProperty runtimeNodeTemplate;
	private SerializedProperty subGraphways;
	private SerializedProperty toConnectNodeA;
	private SerializedProperty toConnectNodeB;
	private bool nodePlacementEnabled = false;

	private Graphway origin;

	void OnEnable()
	{
		// Assign properties
		nodeColor = serializedObject.FindProperty("nodeColor");	
		subnodeColor = serializedObject.FindProperty("subnodeColor");	
		nodeSize = serializedObject.FindProperty("nodeSize");	
		subnodeSize = serializedObject.FindProperty("subnodeSize");	
		arrowSize = serializedObject.FindProperty("arrowSize");
        pathfindFrameLimit = serializedObject.FindProperty("pathfindFrameLimit");
        runtimeNodeTemplate = serializedObject.FindProperty("runtimeNodeTemplate");
        subGraphways = serializedObject.FindProperty("subGraphways");
        toConnectNodeA = serializedObject.FindProperty("toConnectNodeA");
        toConnectNodeB = serializedObject.FindProperty("toConnectNodeB");
        
        origin = target as Graphway;
	}
	
	void OnDisable()
    {
        // Hide Graph
        Graphway graphway = (Graphway)target;
        
        if (graphway != null)
        {
	        DisableRenderers(graphway.transform);

	        foreach (var subGraphway in origin.subGraphways)
				DisableRenderers(subGraphway);
        }
    }
	
    public override void OnInspectorGUI()
	{     
		CreateLogo();
		
		// Create property edit inputs
		serializedObject.Update();
		
		EditorGUILayout.Space();
        EditorGUILayout.PropertyField(nodeColor);
        EditorGUILayout.PropertyField(subnodeColor);
        EditorGUILayout.Space();
        EditorGUILayout.Slider(nodeSize, 0.01f, 100);
        EditorGUILayout.Slider(subnodeSize, 0.05f, 50);
        EditorGUILayout.Slider(arrowSize, 0.025f, 25);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(pathfindFrameLimit);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(runtimeNodeTemplate);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(subGraphways);
        EditorGUILayout.Space();
        DrawConnectionMenu();
        
        serializedObject.ApplyModifiedProperties();

        // Enables/Disable node placement button
		if (nodePlacementEnabled)
		{
			EditorGUILayout.HelpBox("Select the Disable Node Placement button below to end node placement.", MessageType.Info);
			
			if (GUILayout.Button("Disable Node Placement"))
			{
				nodePlacementEnabled = false;
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Select the Enable Node Placement button below to begin placing nodes.", MessageType.Info);
			
			if (GUILayout.Button("Enable Node Placement"))
			{
				nodePlacementEnabled = true;
			}
		}
		
		EditorGUILayout.Space();
	}

    private void DrawConnectionMenu()
    {
	    if (origin.subGraphways.Count >= 2 && 
            origin.subGraphways.All(subGraphway => subGraphway != null))
        {
	        EditorGUILayout.PropertyField(toConnectNodeA);
	        EditorGUILayout.PropertyField(toConnectNodeB);

	        if (origin.toConnectNodeA != null && 
	            origin.toConnectNodeB != null)
	        {
		        var errorTextStyle = new GUIStyle { normal = { textColor = Color.red }};

		        if (origin.toConnectNodeA == origin.toConnectNodeB)
		        {
			        GUILayout.Label("Selected the same node!", errorTextStyle);
		        }
		        else
		        {
			        var graphwayNodeA = FindGraphwayParent(origin.toConnectNodeA.transform);
			        var graphwayNodeB = FindGraphwayParent(origin.toConnectNodeB.transform);

			        if (graphwayNodeA == graphwayNodeB)
			        {
				        GUILayout.Label("Nodes must be in different Graphways. \n" +
				                        "To connect in one Graphway, use the standard connection method.", errorTextStyle);
			        }
			        else if (origin.subGraphways.Contains(graphwayNodeA) == false || 
			                 origin.subGraphways.Contains(graphwayNodeB) == false)
			        {
				        GUILayout.Label("Selected nodes are not part of subgraphways!", errorTextStyle);
			        }
			        else
			        {
				        var nodeIDA = origin.toConnectNodeA.nodeID;
				        var nodeIDB = origin.toConnectNodeB.nodeID;
				        
				        if (NodesAreConnected(nodeIDA, nodeIDB)) 
				        {
					        if (GUILayout.Button("Disconnect Nodes"))
					        {
						        DisconnectNodes(nodeIDA, nodeIDB);
					        }
				        }
						else 
				        {
					        if (GUILayout.Button("Connect Nodes (Bidirectional)"))
					        {
						        ConnectNodes(nodeIDA, nodeIDB, GraphwayConnectionTypes.Bidirectional);
					        }
					        else if (GUILayout.Button("Connect Nodes (Unidirectional A To B)"))
					        {
						        ConnectNodes(nodeIDA, nodeIDB, GraphwayConnectionTypes.UnidirectionalAToB);
					        }
					        else if (GUILayout.Button("Connect Nodes (Unidirectional B To A)"))
					        {
						        ConnectNodes(nodeIDA, nodeIDB, GraphwayConnectionTypes.UnidirectionalBToA);
					        }
				        }
			        }
		        }
	        }
        }
	    
	    EditorGUILayout.Space();
    }

	private void OnSceneGUI()
	{

		if (nodePlacementEnabled)
		{
			// Disable left click selection on other game objects
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

			// Handle mouse click event
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				// Raycast to find hit position
                // NOTE - objects must have colliders attached!
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                if (Physics.Raycast(worldRay, out RaycastHit hit))
				{
					// Add new node at position
					CreateNodeObject(hit.point);

					// Mark scene as dirty to trigger 'Save Changes' prompt
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}

				// Use up event
				Event.current.Use();
			}
		}
		else
		{
			// Re-enable left click selection on other game objects
			HandleUtility.Repaint();
		}

		// Update graph display
		DrawGraph(origin.transform);
		
		foreach (var subGraphway in origin.subGraphways)
			DrawGraph(subGraphway);
	}

	private bool NodesAreConnected(int nodeIDA, int nodeIDB)
	{
		return origin.GraphwayConnector.NodesAreConnected(origin, nodeIDA, nodeIDB);
	}

	public static void DrawGraph(Transform transform)
	{
		Graphway graphway = FindGraphwayParent(transform);
		DrawGraph(graphway);
	}

	public static void DrawGraph(Graphway graphway)
	{
		if (graphway != null)
		{
			// Check integrity of Graphway structure
			CheckHierarchyIntegrity(graphway);
			
			// Enable node renderers
			EnableRenderers(graphway.transform);
			
	        // Draw NODES
			foreach (Transform node in graphway.transform.Find("Nodes").transform)
			{
                node.GetComponent<Renderer>().sharedMaterial.color = graphway.nodeColor;
				node.localScale = new Vector3(graphway.nodeSize, graphway.nodeSize, graphway.nodeSize);
				
				CreateLabel(node.position, graphway.nodeSize, NODE_FONT_SIZE, graphway.nodeColor, node.name);
			}
	
			// Draw CONNECTION & SUBNODES
			foreach (Transform connection in graphway.transform.Find("Connections").transform)
			{
				// Get node IDs of connected nodes
	            int nodeIDA = connection.GetComponent<GraphwayConnection>().nodeIDA; 
	            int nodeIDB = connection.GetComponent<GraphwayConnection>().nodeIDB;
                bool isDisabled = connection.GetComponent<GraphwayConnection>().disabled;

                GraphwayConnectionTypes connectionType = connection.GetComponent<GraphwayConnection>().connectionType;
	
				// Set positions of connected nodes
				graphway.TryFindNodePosition(nodeIDA, out Vector3 nodeAPosition);
				graphway.TryFindNodePosition(nodeIDB, out Vector3 nodeBPosition);
				Vector3 lastPosition = nodeAPosition;
	
				// Check if connection uses subnodes
	            if (connection.childCount > 0)
	            {
	                foreach (Transform subnode in connection.transform)
	                {
	                    // Create subnode
	                    subnode.GetComponent<Renderer>().sharedMaterial.color = graphway.subnodeColor;
						subnode.localScale = new Vector3(graphway.subnodeSize, graphway.subnodeSize, graphway.subnodeSize);
				
                        CreateLabel(subnode.position, graphway.subnodeSize, SUBNODE_FONT_SIZE, graphway.subnodeColor, subnode.name);
	
						// Draw connection to first node or previous subnode
	                    Handles.color = (subnode.GetSiblingIndex() == 0 
	                        ? graphway.nodeColor 
	                        : graphway.subnodeColor
	                    );
	                    
	                    // Check if connection is unidirectional
	                    // If so draw an arrow to indicate direction of allowed movement
	                    if (connectionType == GraphwayConnectionTypes.UnidirectionalAToB)
		                {
		                    Vector3 arrowPosition = (subnode.position + lastPosition) / 2;
		                    Vector3 relativePos = subnode.position - lastPosition;
		                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		
		                    Handles.ConeHandleCap(0, arrowPosition, rotation, graphway.arrowSize, EventType.Repaint);
						}
						else if (connectionType == GraphwayConnectionTypes.UnidirectionalBToA)
		                {
		                    Vector3 arrowPosition = (subnode.position + lastPosition) / 2;
		                    Vector3 relativePos = lastPosition - subnode.position;
		                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		
		                    Handles.ConeHandleCap(0, arrowPosition, rotation, graphway.arrowSize, EventType.Repaint);
						}
				
						// Draw a line to complete the connection
						if (isDisabled)
						{
	                        Handles.DrawDottedLine(lastPosition, subnode.position, 4);
						}
						else
						{
							Handles.DrawLine(lastPosition, subnode.position);
						}
						
                        // Step forward to next subnode/node
	                    lastPosition = subnode.position;
	                }
	            }
	
				// Draw connection to remaining node
	            Handles.color = graphway.nodeColor;
	            
	            // Check if connection is unidirectional
                // If so display arrow showing which direction
	            if (connectionType == GraphwayConnectionTypes.UnidirectionalAToB)
                {
                    Vector3 arrowPosition = (nodeBPosition + lastPosition) / 2;
                    Vector3 relativePos = nodeBPosition - lastPosition;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                    Handles.ConeHandleCap(0, arrowPosition, rotation, graphway.arrowSize, EventType.Repaint);
				}
				else if (connectionType == GraphwayConnectionTypes.UnidirectionalBToA)
                {
                    Vector3 arrowPosition = (nodeBPosition + lastPosition) / 2;
                    Vector3 relativePos = lastPosition - nodeBPosition;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

                    Handles.ConeHandleCap(0, arrowPosition, rotation, graphway.arrowSize, EventType.Repaint);
				}

                // Draw a line to complete the connection
                if (isDisabled)
				{
                    Handles.DrawDottedLine(lastPosition, nodeBPosition, 4);
				}
				else
				{
					Handles.DrawLine(lastPosition, nodeBPosition);
				}	
			}
		}
	}
	
	private static void CheckHierarchyIntegrity(Graphway graphway)
	{
		// Create a list of node IDs
		var nodes = new List<GraphwayNode>(graphway.GetAllNodes());

		foreach (var subGraphway in graphway.subGraphways)
		{
			nodes.AddRange(subGraphway.GetAllNodes());
		}
		
		List<int> nodeIDs = nodes.Select(node => node.nodeID).ToList();

		// Check connection nodes exist
		foreach (Transform connection in graphway.transform.Find("Connections").transform)
		{
            int nodeIDA = connection.GetComponent<GraphwayConnection>().nodeIDA; 
            int nodeIDB = connection.GetComponent<GraphwayConnection>().nodeIDB;

            if (nodeIDs.Contains(nodeIDA) == false || 
                nodeIDs.Contains(nodeIDB) == false)
            {
                DestroyImmediate(connection.gameObject);
            }
		}
	}
	
	private static void CreateLabel(Vector3 basePosition, float baseSize, int fontSize, Color color, string text)
	{
		// Calculate label position
		Vector3 labelPosition = basePosition;
		labelPosition.y += baseSize * 1.5f;

		// Set label style
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.fontSize = fontSize;
		labelStyle.normal.textColor = color;
		labelStyle.alignment = TextAnchor.MiddleCenter;

		// Draw label
		Handles.Label(labelPosition, text, labelStyle);
	}
	
	private void CreateNodeObject(Vector3 nodePosition)
	{
		Graphway graphway = (Graphway)target;
		
		// Create new node object
		GameObject nodeObject = Instantiate(Resources.Load("Prefabs/GraphwayNode")) as GameObject;
		nodeObject.name = graphway.editorNodeCounter.ToString();
		nodeObject.transform.position = nodePosition;
		nodeObject.transform.parent = graphway.transform.Find("Nodes").transform;
        nodeObject.gameObject.AddComponent<GraphwayNode>().SetNodeID(graphway.editorNodeCounter);

		// Register undo operation
		Undo.RegisterCreatedObjectUndo(nodeObject, "Created Node");
		
		graphway.editorNodeCounter++;
	}

	public static void EnableRenderers(Transform transform)
	{
		Graphway graphway = FindGraphwayParent(transform);
		
		SetRenderers(graphway.transform, true);
	}
	
	public static void DisableRenderers(Transform transform)
	{
		Graphway graphway = FindGraphwayParent(transform);
		DisableRenderers(graphway);
	}
	
	public static void DisableRenderers(Graphway graphway)
	{
		SetRenderers(graphway.transform, false);
	}

	private static void SetRenderers(Transform transform, bool enabled)
	{
		if (transform.GetComponent<Renderer>())
		{
			transform.GetComponent<Renderer>().enabled = enabled;
		}
		
		if (transform.childCount > 0)
		{
			foreach (Transform child in transform)
			{
				SetRenderers(child.transform, enabled);
			}
		}
	}

	private void ConnectNodes(int nodeIDA, int nodeIDB, GraphwayConnectionTypes connectionType)
	{
		origin.GraphwayConnector.ConnectSelectedNodes(origin, nodeIDA, nodeIDB, connectionType);
	}
	
	private void DisconnectNodes(int nodeIDA, int nodeIDB)
	{
		origin.GraphwayConnector.DisconnectNodes(origin, nodeIDA, nodeIDB);
	}

    /// <summary>
    /// Finds the parent object from a Graphway child.
    /// </summary>
    /// <param name="transform">The Transform of the child object.</param>
    /// <returns>Parent transform or null if doesn't exist.</returns>
	public static Graphway FindGraphwayParent(Transform transform)
	{
		Transform t = transform;
		
		if (transform.GetComponent<Graphway>())
		{
			return transform.GetComponent<Graphway>();
		}
		else
		{
			while (t.parent != null)
			{
				if (t.parent.GetComponent<Graphway>())
				{
					return t.parent.GetComponent<Graphway>();
				}
				
				t = t.parent.transform;
			}
			
			return null;
		}
	}
	
	public static void CreateLogo()
	{
		EditorGUILayout.Space();
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		
		GUILayout.Label(Resources.Load("Textures/GraphwayLogo") as Texture, style, GUILayout.ExpandWidth(true));
	}
}