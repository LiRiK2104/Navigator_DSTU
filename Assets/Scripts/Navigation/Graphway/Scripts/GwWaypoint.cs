using UnityEngine;

public class GwWaypoint
{
    public Vector3 position;
    public int? nodeID;
    public float speed;

    public GwWaypoint(Vector3 position, int? nodeID = null, float speed = 1)
    {
        this.position = position;
        this.nodeID = nodeID;
        this.speed = speed;
    }
}