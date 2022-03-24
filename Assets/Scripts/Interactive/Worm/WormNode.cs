using UnityEngine;

public class WormNode : MonoBehaviour , IPoolable
{
    public Vector2 LastPos => lastPos; 
    public Vector2 lastPos;
    private WormNode followNode;
    public void SetFollowNode(WormNode followNode)
    {
        this.followNode = followNode;
    }
    public void Move()
    {
        lastPos = transform.position;
        transform.position = followNode.lastPos;
    }

    public void Init()
    {
        
    }

    public void WhenPooled()
    {
        followNode = null;
        lastPos = Vector2.zero;
    }
}