using UnityEngine;

[RequireComponent(typeof(GraphwayConnector))]
public class GraphwayNode : MonoBehaviour 
{
	private GraphwayConnector _graphwayConnector;
	public int nodeID = -1;

	public GraphwayConnector GraphwayConnector
	{
		get
		{
			if (_graphwayConnector == null)
				_graphwayConnector = GetComponent<GraphwayConnector>();

			return _graphwayConnector;
		}
	}

	public void SetNodeID(int nodeID)
	{
		this.nodeID = nodeID;
	}
}
