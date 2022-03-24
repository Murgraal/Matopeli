using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

public class Worm : MonoBehaviour
{
    public event Action<PlayerInteractionType> OnInteraction;
    
    [SerializeField] private WormNode nodePrefab;
    [SerializeField] private Transform nodeParent;
    
    private Vector2 direction = Vector2.up;
    private Vector2 nextDirection;
    
    private int expectedWormLength = 1;

    private WormNode head => wormNodes[0];
    private List<WormNode> wormNodes = new List<WormNode>();
    private WaitForSeconds waitForInterval;

    private MonoObjectPool<WormNode> nodePool;

    private IEnumerator Move()
    {
        var endOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            if (direction != nextDirection)
            {
                direction = nextDirection;
            }

            MoveHead();
            MoveBody();

            if (wormNodes.Count < expectedWormLength)
            {
                AddNode();
            }

            yield return waitForInterval;
            yield return endOfFrame;
        }
    }

    public void InitWorm(float startMoveInterval)
    {
        waitForInterval = new WaitForSeconds(startMoveInterval);
        wormNodes.Add(GetComponent<WormNode>());
        nextDirection = direction;
        nodePool = new MonoObjectPool<WormNode>(nodePrefab);
        
        StartCoroutine(Move());
    }

    public void Reset()
    {
        if (wormNodes.Count > 1)
        {
            nodePool.PoolAll();
            wormNodes.Clear();
        }
    }

    public List<Vector2> GetAllNodePositions()
    {
        var result = new List<Vector2>();

        foreach (var node in wormNodes)
        {
            result.Add(node.gameObject.transform.position);
        }

        return result;
    }

    private void MoveHead()
    {
        head.lastPos = transform.position;
        transform.position += (Vector3) direction.normalized;
    }

    private void MoveBody()
    {
        foreach (var node in wormNodes)
        {
            if (node == head) continue;
            node.Move();
        }
    }

    private void AddNode()
    {
        var lastChildLastPos = wormNodes[^1].LastPos;

        var newNode = nodePool.Spawn(
            lastChildLastPos,
            Quaternion.identity,
            nodeParent
        );

        newNode.SetFollowNode(wormNodes[^1]);

        wormNodes.Add(newNode);
    }

    public void GrowWorm() //This method is kinda pointless
    {
        expectedWormLength++;
    }

    public void SetMoveInterval(float newInterval)
    {
        waitForInterval = new WaitForSeconds(newInterval);
    }

    public void TurnWorm(Vector2 newDir)
    {
        if (newDir == -direction) return;
        nextDirection = newDir;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Pickup"))
        {
            OnInteraction?.Invoke(PlayerInteractionType.PickedUpItem);
        }
        else
        {
            OnInteraction?.Invoke(PlayerInteractionType.HitWall);
        }
    }
}