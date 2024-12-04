using System.Collections.Generic;
using UnityEngine;

public class EnemyRobotMovement : EnemyMovement
{
    private EnemyShoot _enemyShoot;

    protected override void Awake()
    {
        IsDamaged = false;
        _enemyShoot = GetComponent<EnemyShoot>();
    }

    protected override void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _player = _playerAwarenessController.getPlayer();
        _lastPlayerPosition = _player.position;
        _grid = GridMap.grid;
        StartCoroutine(RecalculatePathPeriodically());
        CalculatePath();
    }

    protected override void FollowPath(List<Node> path)
    {
        if (path == null || path.Count == 0)
            return;

        // Get the next node in the path.
        Node nextNode = path[0];

        Vector2 targetPosition = GridMap.GridToWorldPosition(nextNode);

        Node nextNodeOnGrid = GridMap.WorldToGridPosition((int)nextNode.x, (int)nextNode.y);
        Node currentNode = GridMap.WorldToGridPosition(transform.position);

        // Obstacle avoidance 
        // If only one adjacent node is an obstacle, temporarily move horizontally or vertically.
        if (HasOneSharedAdjacentObstacle(currentNode, nextNodeOnGrid) && GridMap.IsDiagonal(currentNode, nextNodeOnGrid) /*&& (int)Vector2.Distance(transform.position, targetPosition) == 1*/)
        {
            // Move towards the target in either the horizontal or vertical direction.
            Dictionary<string, Node> sharedAdjacentNodes = GetSharedAdjacentNodes(currentNode, nextNodeOnGrid);
            if (sharedAdjacentNodes["horizontal"].walkable && !path.Contains(sharedAdjacentNodes["horizontal"]))
            {
                path.Insert(0, sharedAdjacentNodes["horizontal"]);
                nextNode = path[0];
            }
            else if (sharedAdjacentNodes["vertical"].walkable && !path.Contains(sharedAdjacentNodes["vertical"]))
            {
                path.Insert(0, sharedAdjacentNodes["vertical"]);
                nextNode = path[0];

            }
        }
       
        targetPosition = GridMap.GridToWorldPosition(nextNode);

        if (_enemyShoot.WithinFireDistance(transform.position, _player.position) && _enemyShoot.EnemyCanFire() && !((Vector2) transform.position == targetPosition))
        {
            Vector2 playerPositionOnGrid = GridMap.WorldToGridPositionVector(_player.position);
            UpdateTargetDirection(new Node(playerPositionOnGrid.x, playerPositionOnGrid.y, true));
            RotateTowardsTarget();
            _rigidbody.velocity = Vector2.zero;
            return;
        } 
        else
        {
            UpdateTargetDirection(nextNode);
            RotateTowardsTarget();
            _rigidbody.velocity = _targetDirection * Speed;
        }       
        
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            _rigidbody.velocity = Vector2.zero;
            path.RemoveAt(0);
        }
    }
}
