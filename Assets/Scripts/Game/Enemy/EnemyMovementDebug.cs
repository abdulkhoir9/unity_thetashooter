using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementDebug : EnemyMovement
{
    protected override void Awake() { }

    protected override void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _player = _playerAwarenessController.getPlayer();
        _lastPlayerPosition = _player.position;
        _grid = GridMap.grid;
    }

    protected override void FixedUpdate()
    {
        //if (_currentPath != null && _currentPath.Count > 0)
        //{
        //    FollowPath(_currentPath);
        //}
    }

    protected override void Update()
    {
        if (PathfindingDebug.IsCalculatingPath)
        {
            List<Node> path;
            _grid = GridMap.grid;

            // Initialize variables
            Node enemyGridPosition = GridMap.WorldToGridPosition(transform.position);
            Node playerGridPosition = GridMap.WorldToGridPosition(_player.position);

            // Find the path
            path = Pathfinding.FindPath(_grid, enemyGridPosition, playerGridPosition);
        }
    }

    protected override void CalculatePath()
    {
        _grid = GridMap.grid;

        if (PathfindingDebug.IsPaused)
        {
            AudioManager.instance.PlaySFX("ButtonClick");
            PathfindingDebug.ResumePathfindingCalculation();

            return;
        }
        else
        {
            AudioManager.instance.PlaySFX("ButtonClick");
            PathfindingDebug.CancelPathfindingCalculation();
            PathfindingDebug.IsCalculatingPath = true;
            FrameRateStats.ResetStats();
            FrameRateStats.ToggleUpdate(true);

            // Initialize variables
            Node enemyGridPosition = GridMap.WorldToGridPosition(transform.position);
            Node playerGridPosition = GridMap.WorldToGridPosition(_player.position);

            // Find the path
            _currentPath = Pathfinding.FindPathDebug(_grid, enemyGridPosition, playerGridPosition);

            //// Calculate time without debug method
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //Pathfinding.FindPath(_grid, enemyGridPosition, playerGridPosition);
            //stopwatch.Stop();
            //UnityEngine.Debug.Log($"Execution time: {stopwatch.Elapsed.TotalMilliseconds}");

            if (_currentPath == null)
                return;

            //if (_currentPath != null && _currentPath.Count > 0)
            //{
            //    FollowPath(_currentPath);
            //}
        }
    }

    protected override void FollowPath(List<Node> path)
    {
        if (path == null || path.Count == 0)
        {
            return;
        }

        // Get the next node in the path.
        Node nextNode = path[0];

        Vector2 targetPosition = GridMap.GridToWorldPosition(nextNode);

        Node nextNodeOnGrid = GridMap.WorldToGridPosition((int)nextNode.x, (int)nextNode.y);
        Node currentNode = GridMap.WorldToGridPosition(transform.position);

        // Obstacle avoidance for the A* algorithm
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

        // Calculate the direction to the next node.
        UpdateTargetDirection(nextNode);
        RotateTowardsTarget();

        _rigidbody.velocity = _targetDirection * Speed;

        // If the enemy is close to the next node, remove it from the path.
        targetPosition = GridMap.GridToWorldPosition(nextNode);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            _rigidbody.velocity = Vector2.zero;
            path.RemoveAt(0);
        }
    }

    public void CallCalculatePath()
    {
        CalculatePath();
    }
}
