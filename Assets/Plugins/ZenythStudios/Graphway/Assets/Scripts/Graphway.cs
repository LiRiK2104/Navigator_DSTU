using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(GraphwayConnector))]
public class Graphway : MonoBehaviour 
{
	public static Graphway instance;
	
	[Tooltip("Color of NODES in editor.")]
	public Color nodeColor = new Color32(236, 13, 69, 255);
	
	[Tooltip("Color of SUBNODES in editor.")]
	public Color subnodeColor = new Color32(40, 12, 54, 255);
	
	[Tooltip("Size of NODES in editor.")]
	public float nodeSize = 10;
	
	[Tooltip("Size of SUBNODES in editor.")]
	public float subnodeSize = 5;
	
	[Tooltip("Size of unidirectional connection arrows in editor.")]
	public float arrowSize = 5;

    [Tooltip("Number of nodes that can be checked in a single frame at runtime.  A higher limit will find paths faster, but may impact performance.")]
    public int pathfindFrameLimit = 100;

    public int editorNodeCounter = 0; // Keep public so value is saved upon editor close

    public List<Graphway> subGraphways;
    public GraphwayNode toConnectNodeA;
    public GraphwayNode toConnectNodeB;
    public GwNode runtimeNodeTemplate; 

    // RUNTIME ONLY VARS
    public Dictionary<int, GwNode> nodes;
	private List<GwJob> jobs;
	private GwJob activeJob;
	private List<int> openNodes;
    private List<int> closedNodes;
    private int currentNodeID;
    private GraphwayConnector _graphwayConnector;
    private bool isInitialized;
    
    public GraphwayConnector GraphwayConnector
    {
	    get
	    {
		    if (_graphwayConnector == null)
			    _graphwayConnector = GetComponent<GraphwayConnector>();

		    return _graphwayConnector;
	    }
    }
    
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
	    // Process next job (if there is one)
        if (jobs.Count > 0 && activeJob == null)
        {
            StartCoroutine(ProcessJob(jobs[0]));
        }
    }


    public void Initialize()
    {
	    if (isInitialized)
		    return;
	    
	    // Create new list to store pathfinding jobs
        jobs = new List<GwJob>();

        // Convert GameObject structure to class objects for efficiency
        // Check game objects exist
        if (!transform.Find("Nodes"))
        {
            Debug.LogError("Missing Graphway child object 'Nodes'.");
        }
        else if (!transform.Find("Connections"))
        {
            Debug.LogError("Missing Graphway child object 'Connections'.");
        }
        else
        {
            Transform nodesParent = transform.Find("Nodes").transform;
            Transform connectionsParent = transform.Find("Connections").transform;

            // Create dictionary of NODES
            nodes = new Dictionary<int, GwNode>();

            foreach (Transform node in nodesParent.transform)
            {
                GraphwayNode nodeData = node.GetComponent<GraphwayNode>();

                var runtimeNode = Instantiate(runtimeNodeTemplate, node.position, Quaternion.identity, transform);
                runtimeNode.Initialize(nodeData.nodeID);
                nodes[nodeData.nodeID] = runtimeNode;
            }

            foreach (var subGraphway in subGraphways)
            {
	            subGraphway.Initialize();

	            foreach (var subGraphwayNode in subGraphway.nodes)
	            {
		            nodes[subGraphwayNode.Key] = subGraphwayNode.Value;
	            }
            }

            // Add CONNECTION structure
            foreach (Transform connection in connectionsParent.transform)
            {
                GraphwayConnection connectionData = connection.GetComponent<GraphwayConnection>();

                // Check if connection uses SUBNODES
                Transform[] subnodesAB = null;
                Transform[] subnodesBA = null;

                if (connection.childCount > 0)
                {
                    // Read positions of child objects into array
                    subnodesAB = new Transform[connection.childCount];

                    int i = 0;

                    foreach (Transform subnode in connection.transform)
                    {
                        subnodesAB[i] = subnode;

                        i++;
                    }

                    // Clone & Reverse subnodes fron A->B to B->A
                    subnodesBA = (Transform[])subnodesAB.Clone();

                    Array.Reverse(subnodesBA);
                }

                // Add connection A->B
                if (connectionData.connectionType == GraphwayConnectionTypes.Bidirectional || 
                    connectionData.connectionType == GraphwayConnectionTypes.UnidirectionalAToB)
                {
	                //AddConnection(connectionData.nodeIDA, connectionData.nodeIDB, connectionData, subnodesAB);
	                nodes[connectionData.nodeIDA].AddConnection(connectionData.nodeIDB, connectionData.disabled, connectionData.speedWeight, subnodesAB);
                }

                // Add connection B->A
                if (connectionData.connectionType == GraphwayConnectionTypes.Bidirectional || 
                    connectionData.connectionType == GraphwayConnectionTypes.UnidirectionalBToA)
                {
	                //AddConnection(connectionData.nodeIDB, connectionData.nodeIDA, connectionData, subnodesBA);
                    nodes[connectionData.nodeIDB].AddConnection(connectionData.nodeIDA, connectionData.disabled, connectionData.speedWeight, subnodesBA);
                }
            }

            // Remove graphway child gameobjects as they are no longer needed during runtime
            foreach (var childNode in nodesParent.GetComponentsInChildren<Transform>())
	            childNode.gameObject.SetActive(false);//Destroy(childNode.gameObject);
            
            //Destroy(connectionsParent.gameObject);

            isInitialized = true;
        }
    }
    
    /// <summary>
    /// Find a new path from A to B using Graphway (static method).
    /// </summary>
    /// <param name="origin">Starting point.</param>
    /// <param name="targetPosition">End point.</param>
    /// <param name="callback">Callback method to receive pathfinding result.</param>
    /// <param name="clampToEndNode">Clamp final position to closest node.</param>
    /// <param name="debugMode">Enable Debug Mode to see algorithm in action (slowed down).  ENABLE GIZMOS!</param>
	public static void FindPath(Vector3 origin, Vector3 targetPosition, Action<GwWaypoint[]> callback, bool clampToEndNode = true, bool debugMode = false)
	{
		instance.PathFind(origin, targetPosition, callback, clampToEndNode, debugMode);
	}
    
    /// <summary>
    /// Find a new path from A to B using Graphway.
    /// </summary>
    /// <param name="origin">Starting point.</param>
    /// <param name="targetPosition">End point.</param>
    /// <param name="callback">Callback method to receive pathfinding result.</param>
    /// <param name="availableOrigins">If not null allows only nodes with specified positions.</param>
    /// <param name="clampToEndNode">Clamp final position to closest node.</param>
    /// <param name="debugMode">Enable Debug Mode to see algorithm in action (slowed down).  ENABLE GIZMOS!</param>
    public static void FindPath(Vector3 origin, Vector3 targetPosition, Action<GwWaypoint[]> callback, int[] availableOrigins, bool clampToEndNode = true, bool debugMode = false)
    {
	    instance.PathFind(origin, targetPosition, callback, availableOrigins, clampToEndNode, debugMode);
    }

    /// <summary>
    /// Find a new path from A to B using Graphway.
    /// </summary>
    /// <param name="origin">Starting point.</param>
    /// <param name="targetPosition">End point.</param>
    /// <param name="callback">Callback method to receive pathfinding result.</param>
    /// <param name="clampToEndNode">Clamp final position to closest node.</param>
    /// <param name="debugMode">Enable Debug Mode to see algorithm in action (slowed down).  ENABLE GIZMOS!</param>
    public void PathFind(Vector3 origin, Vector3 targetPosition, Action<GwWaypoint[]> callback, bool clampToEndNode = true, bool debugMode = false)
    {
        // Create new job and add it to the queue
        jobs.Add(new GwJob(origin, targetPosition, callback, clampToEndNode, debugMode));
    }
    
    /// <summary>
    /// Find a new path from A to B using Graphway.
    /// </summary>
    /// <param name="origin">Starting point.</param>
    /// <param name="targetPosition">End point.</param>
    /// <param name="callback">Callback method to receive pathfinding result.</param>
    /// <param name="availableOrigins">If not null allows only nodes with specified positions.</param>
    /// <param name="clampToEndNode">Clamp final position to closest node.</param>
    /// <param name="debugMode">Enable Debug Mode to see algorithm in action (slowed down).  ENABLE GIZMOS!</param>
    public void PathFind(Vector3 origin, Vector3 targetPosition, Action<GwWaypoint[]> callback, int[] availableOrigins, bool clampToEndNode = true, bool debugMode = false)
    {
	    // Create new job and add it to the queue
	    jobs.Add(new GwJob(origin, targetPosition, callback, clampToEndNode, debugMode, availableOrigins));
    }

    public List<GraphwayNode> GetNodes()
    {
	    return transform.Find("Nodes").transform.GetComponentsInChildren<GraphwayNode>().ToList();
    }

    public bool TryFindNodePosition(int nodeID, out Vector3 nodePosition)
    {
	    nodePosition = default;
	    var node = transform.Find("Nodes/" + nodeID);

	    if (node != null)
	    {
		    nodePosition = node.position;   
		    return true;
	    }

	    foreach (var subGraphway in subGraphways)
	    {
		    if (subGraphway.TryFindNodePosition(nodeID, out nodePosition))
			    return true;
	    }

	    return false;
    }

    private bool TryGetNode(int nodeID, out GwNode node)
    {
	    node = null;
	    
	    if (nodes.ContainsKey(nodeID))
	    {
		    node = nodes[nodeID];
		    return true;
	    }

	    foreach (var subGraphway in subGraphways)
	    {
		    if (subGraphway.TryGetNode(nodeID, out node) && node != null)
			    return true;
	    }

	    return false;
    }

    private IEnumerator ProcessJob(GwJob job)
    {
        activeJob = job;

        // Find the closest start node
        int startNodeID = ClosestNodeID(activeJob.origin, activeJob.availableOrigins);
        
        // Find the closest end node
        int targetNodeID = ClosestNodeID(activeJob.targetPosition);
        
        // Check start/end nodes were found
        if (startNodeID == -1 || targetNodeID == -1)
        {
            activeJob.callback(null);
        }
        else
        {
	        // Reset pathfinding vars
	        foreach (KeyValuePair<int, GwNode> node in nodes)
	        {
	            node.Value.ResetPathfindingVars();
	        }
	        
	        // Find path using A* algorithm
	        bool pathFound = false;
	        
            // Create open nodes list and add start node
	        openNodes = new List<int>();
	        openNodes.Add(startNodeID);
	        
            // Create closed nodes list
	        closedNodes = new List<int>();
	        
            // Reset current node var
	        currentNodeID = -1;

            // Create counter to enforce frame resource limit
            int nodeCounter = 1;

	        while (openNodes.Count > 0 && pathFound == false)
	        {
	            // Find node in open list with lowest fCost
	            float lowestCost = -1;
	            
	            foreach (int openNode in openNodes)
	            {
	                if (lowestCost == -1 || nodes[openNode].FCost() < lowestCost)
	                {
	                    currentNodeID = openNode;
	                    
	                    lowestCost = nodes[openNode].FCost();
	                }
	            }
	            
	            // Remove current node from open list and add it to closed list
	            openNodes.Remove(currentNodeID);
	            closedNodes.Add(currentNodeID);
	            
	            // Check if current node is the target node
	            if (currentNodeID == targetNodeID)
	            {
	                pathFound = true;
	            }
	            else
	            {
                    // Get list of nodes connected to the current one
	                foreach (KeyValuePair<int, GwConnection> connection in nodes[currentNodeID].connections)
	                { 
	                    int connectedNodeID = connection.Value.connectedNodeID;
	                    
                        // Check connection is enabled and node has not already been checked
	                    if (connection.Value.disabled == false && closedNodes.Contains(connectedNodeID) == false)
	                    {
                            // Calculate g/h/f
	                        float gCost = nodes[currentNodeID].gCost + (CalculatePathLength(currentNodeID, connectedNodeID) / connection.Value.speedWeight);
	                        float hCost = Vector3.Distance(nodes[connectedNodeID].transform.position, nodes[targetNodeID].transform.position);
	                        float fCost = gCost + hCost;
	                        
                            // Update open list if node already exists and movement cost is less
	                        if (openNodes.Contains(connectedNodeID) && fCost < nodes[connectedNodeID].FCost())
	                        {
		                        nodes[connectedNodeID].gCost = gCost;
		                        nodes[connectedNodeID].hCost = hCost;
		                        nodes[connectedNodeID].parentNodeID = currentNodeID;
	                        }
                            // Otherwise add node to open list if not already on it
                            else if(!openNodes.Contains(connectedNodeID))
                            {
	                            nodes[connectedNodeID].gCost = gCost;
	                            nodes[connectedNodeID].hCost = hCost;
	                            nodes[connectedNodeID].parentNodeID = currentNodeID;
	                            
	                            openNodes.Add(connectedNodeID);
	                        }
	                    }
	                }
	            }
	           
                // Check if reach frame process limit
	            if (activeJob.debugMode)
	            {
	                yield return new WaitForSeconds(0.25f);
	            }
                else if (nodeCounter % pathfindFrameLimit == 0)
                {
                    yield return null;
                }

                nodeCounter++;
            }
	        
            // Pause if debug mode is on to allow time to review path
	        if (activeJob.debugMode)
	        {
	            yield return new WaitForSeconds(1);
	        }
	        
            // Send result back to callback method
	        if (pathFound)
	        {
	           GwWaypoint[] path = TracePath(activeJob.targetPosition, currentNodeID);
	           
	           activeJob.callback(path);
	        }
	        else
	        {
	            activeJob.callback(null);
	        }
        }
        
        
        // Finish & Remove job
        jobs.RemoveAt(0);
        
        activeJob = null;
    }
    
    /// <summary>
    /// Finds the closest node to a world position.
    /// </summary>
    /// <param name="position">Target position.</param>
    /// /// <param name="availableIds">If not null allows only nodes with specified positions.</param>
    /// <returns>The ID of the closest node.</returns>
    private int ClosestNodeID(Vector3 position, int[] availableIds = null)
    {
        int closestNodeID = -1;
        
        float closestNodeDistance = 0;
        
        foreach (KeyValuePair<int, GwNode> node in nodes)
        {
	        if ( availableIds != null && 
	             availableIds.Length > 0 && 
	             availableIds.Contains(node.Value.nodeID) == false)
		        continue;
	        
            float distance = Vector3.Distance(position, node.Value.transform.position);
            
            if (closestNodeID == -1 || distance < closestNodeDistance)
            {
                closestNodeID = node.Value.nodeID;
                
                closestNodeDistance = distance;
            }
        }
        
        return closestNodeID;
    }

    /// <summary>
    /// Calculates the length of a connection in world space taking into account subnodes.
    /// </summary>
    /// <param name="nodeIDFrom">Node A ID.</param>
    /// <param name="nodeIDTo">Node B ID.</param>
    /// <returns>The legnth of a connection.</returns>
    private float CalculatePathLength(int nodeIDFrom, int nodeIDTo)
    {
        float pathLength = 0;
        
        // Find connection data
        GwConnection connection = nodes[nodeIDFrom].connections[nodeIDTo];
        
        // Check if connection path contains subnodes
        Vector3 currentPosition = nodes[nodeIDFrom].transform.position;
        
        if (connection.subnodes != null && connection.subnodes.Length > 0)
        {
            for (int i = 0 ; i < connection.subnodes.Length ; i++)
            {
                pathLength += Vector3.Distance(currentPosition, connection.subnodes[i].position);
                
                currentPosition = connection.subnodes[i].position;
            }
        }
        
        // Add path length to final node
        pathLength += Vector3.Distance(currentPosition, nodes[nodeIDTo].transform.position);
        
        return pathLength;
    }
    
    /// <summary>
    /// Traces the path from the target node back to the start node then reverses it.
    /// </summary>
    /// <param name="targetPosition">The target end point.</param>
    /// <param name="currentNodeID">The target node ID.</param>
    /// <returns>Waypoints array for path from A to B.</returns>
    private GwWaypoint[] TracePath(Vector3 targetPosition, int currentNodeID)
    {
         // Trace path
        List<GwWaypoint> path = new List<GwWaypoint>();

		// Add waypoint to target position (if off graph)
		if (activeJob.clampToEndNode == false)
		{
			path.Add(new GwWaypoint(targetPosition));
		}
		
        while (currentNodeID != -1)
        {
            int parentNodeID = nodes[currentNodeID].parentNodeID;

            if (parentNodeID != -1)
            {
                // Add waypoint
                GwConnection connection = nodes[parentNodeID].connections[currentNodeID];
              
                path.Add(new GwWaypoint(nodes[currentNodeID].transform.position, connection.speedWeight));
               
	            // Add subnode waypoints
	            if (connection.subnodes != null && connection.subnodes.Length > 0)
	            {
	                for (int i = connection.subnodes.Length - 1 ; i >= 0 ; i--)
	                {
	                    path.Add(new GwWaypoint(connection.subnodes[i].position, connection.speedWeight));
	                }
	            }
            }
            else
            {
                // Add starting node
                path.Add(new GwWaypoint( nodes[currentNodeID].transform.position));
            }
            
            // Update current node
            currentNodeID = parentNodeID;
        }
        
        // Reverse path from B->A to A->B
        path.Reverse();
        
        return path.ToArray();
    }
    
    /// <summary>
    /// Debug algorithm using Gizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        // DRAW GRAPH
        if (Application.isPlaying)
        {
	        foreach (KeyValuePair<int, GwNode> node in nodes)
	        {
	            // Draw node
	            Gizmos.color = nodeColor;
	            Gizmos.DrawSphere(RaiseHeight(node.Value.transform.position), nodeSize / 2);
	         
	            // Draw ACTIVE Connections
	            foreach (KeyValuePair<int, GwConnection> connection in node.Value.connections)
	            {
	                if (connection.Value.disabled == false && 
	                    TryGetNode(connection.Value.connectedNodeID, out GwNode nodeB))
	                {
		                // Set positions of connected nodes
		                Vector3 nodeAPosition = node.Value.transform.position;
			            Vector3 nodeBPosition = nodeB.transform.position;
			            Vector3 lastPosition = nodeAPosition;
			
						// Traverse subnodes (if any)
			            if (connection.Value.subnodes != null && connection.Value.subnodes.Length > 0)
			            {
			                for (int i = 0 ; i < connection.Value.subnodes.Length ; i++)
			                {
			                    Vector3 subnodePosition = connection.Value.subnodes[i].position;
			                    
			                    // Draw subnode
			                    Gizmos.color = subnodeColor;
			                    Gizmos.DrawSphere(RaiseHeight(subnodePosition), subnodeSize / 2);
			                    
								// Draw connection to first node or previous subnode
			                    Gizmos.color = (i == 0 ? nodeColor : subnodeColor);
			                    Gizmos.DrawLine(RaiseHeight(lastPosition), RaiseHeight(subnodePosition));
			                    
			                    // Draw arrow if connection is Unidirectional
			                    if ( ! nodes[connection.Value.connectedNodeID].connections.ContainsKey(node.Value.nodeID))
			                    {
			                        DrawArrow(RaiseHeight(lastPosition), RaiseHeight(subnodePosition - lastPosition) / 2, (i == 0 ? nodeColor : subnodeColor));
			                    }
			                    
	                            lastPosition = subnodePosition;
			                }
			            }
			
						// Draw connection to remaining node
			            Gizmos.color = nodeColor;
						Gizmos.DrawLine(RaiseHeight(lastPosition), RaiseHeight(nodeBPosition));
						
						// Draw arrow if connection is Unidirectional
	                    if ( ! nodes[connection.Value.connectedNodeID].connections.ContainsKey(node.Value.nodeID))
	                    {	
							DrawArrow(RaiseHeight(lastPosition), RaiseHeight(nodeBPosition - lastPosition) / 2, nodeColor);
						}
					}
	            }
	        }
	        
	        
	        if (activeJob != null && activeJob.debugMode)
	        {
	            int parentID;
	            
	            // Draw OPEN nodes
	            for (int i = 0 ; i < openNodes.Count ; i++)
	            {
	                int nodeID = openNodes[i];
	                
	                Gizmos.color = Color.white;
                    Gizmos.DrawSphere(nodes[nodeID].transform.position, nodeSize);
                    
                    // Draw connection to parent
                    parentID = nodes[nodeID].parentNodeID;
                    
                    if (parentID != -1)
                    {
						Gizmos.DrawLine(RaiseHeight(nodes[nodeID].transform.position), RaiseHeight(nodes[parentID].transform.position));
                    }
	            }
	            
	            // Draw CLOSED nodes
	            for (int i = 0 ; i < closedNodes.Count ; i++)
	            {
	                int nodeID = closedNodes[i];
	                
	                Gizmos.color = Color.gray;
                    Gizmos.DrawSphere(nodes[nodeID].transform.position, nodeSize);
                    
                    // Draw connection to parent
                    parentID = nodes[nodeID].parentNodeID;
                    
                    if (parentID != -1)
                    {
						Gizmos.DrawLine(RaiseHeight(nodes[nodeID].transform.position), RaiseHeight(nodes[parentID].transform.position));
                    }
	            }
	            
	            // Draw ACTIVE node
	            Gizmos.color = Color.green;
                Gizmos.DrawSphere(nodes[currentNodeID].transform.position, nodeSize);
                
                // Draw connection to parent
                parentID = nodes[currentNodeID].parentNodeID;
                
                if (parentID != -1)
                {
					Gizmos.DrawLine(RaiseHeight(nodes[currentNodeID].transform.position), RaiseHeight(nodes[parentID].transform.position));
                }
	        }
	    }
    }

    /// <summary>
    /// Used by OnDrawGizmos.
    /// </summary>
    /// <param name="position">Arrow origin.</param>
    /// <param name="direction">Arrow direction.</param>
    /// <param name="color">Arrow color.</param>
    private void DrawArrow(Vector3 position, Vector3 direction, Color color)
	{
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+30,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-30,0) * new Vector3(0,0,1);
		
		Gizmos.DrawRay(position + direction, right * (nodeSize / 2));
		Gizmos.DrawRay(position + direction, left * (nodeSize / 2));
	}

    /// <summary>
    /// Used by OnDrawGizmos to add extra height so line do not intersect meshes. 
    /// </summary>
    /// <param name="position">Position to add height to.</param>
    /// <returns>Adjusted height.</returns>
    private Vector3 RaiseHeight(Vector3 position)
	{
		position.y += 0.01f;
		
		return position;
	}
}