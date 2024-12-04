using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [field: SerializeField]
    public float Speed { get; set; }

    [field: SerializeField]
    public float RotationSpeed { get; set; }

    protected Rigidbody2D _rigidbody;
    protected PlayerAwarenessController _playerAwarenessController;

    [SerializeField]
    protected Transform _player;
    protected Vector2 _targetDirection;

    protected Node[,] _grid;

    protected List<Node> _currentPath;

    protected Vector2 _lastPlayerPosition;

    [SerializeField]
    private float _pathRecalculationInterval;

    public bool IsDamaged { get; set; }

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        //_player = _playerAwarenessController.getPlayer();

        IsDamaged = false;
    }

    protected virtual void Start()
    {
        _lastPlayerPosition = _player.position;

        //Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        //foreach (Tilemap tilemap in tilemaps)
        //{
        //    if (tilemap.gameObject.CompareTag("Floor"))
        //    {
        //        _gridMap = tilemap.GetComponent<GridMap>();
        //        _grid = GridMap.grid;
        //        //minX = tilemap.cellBounds.xMin;
        //        //minY = tilemap.cellBounds.yMin;
        //        //Debug.Log("minX, minY = " + minX + ", " + minY);

        //        break;
        //    }
        //}
        _grid = GridMap.grid;

        //Node enemyGridPosition = _gridMap.GetNodeAtPosition(10, 13);
        //Node playerGridPosition = _gridMap.GetNodeAtPosition(11, 13);
        //Debug.Log("Sus FindPath... enemyGridPosition = (" + enemyGridPosition.x + ", " + enemyGridPosition.y + ") | playerGridPosition = " + playerGridPosition.x + ", " + playerGridPosition.y + ")");
        //_currentPath = Pathfinding.FindPath(_grid, enemyGridPosition, playerGridPosition);
        //int pathLength = _currentPath.Count;
        //Debug.LogWarning("--------------");
        //string nodeList = "1 - path found (" + pathLength + ") = ";
        //foreach (var node in _currentPath)
        //{
        //    nodeList += "(" + node.x + ", " + node.y + "), ";
        //}
        //Debug.Log(nodeList);

        //Debug.LogError("STOOOOOOOOOOOOOOOOOOOOOOOOPPPPPPPPPPPPPPPPP!!!!!!!!!!!!");
        //Debug.Break();

        StartCoroutine(RecalculatePathPeriodically());
        CalculatePath();
    }

    protected virtual void FixedUpdate()
    {
        if(IsDamaged)
        {
            _rigidbody.velocity = Vector2.zero;
        }
        else if(_currentPath != null && _currentPath.Count > 0)
        {
            FollowPath(_currentPath);
        }
    }

    protected virtual void Update()
    {
        try
        {
            if (Vector2.Distance(_lastPlayerPosition, _player.position) >= _pathRecalculationInterval)
            {
                if (_lastPlayerPosition == null)
                    Debug.Log("lastPlayerPosition is null");
                CalculatePath();
                _lastPlayerPosition = _player.position;
            }
        }
        catch (MissingReferenceException)
        {
            Debug.Log("Player died, stopping enemy movement...");
            enabled = false;
        }
    }

    protected void UpdateTargetDirection(Node nextNode)
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            Vector2 targetPosition = GridMap.GridToWorldPosition(nextNode);
            _targetDirection = new Vector2((targetPosition.x) - transform.position.x, (targetPosition.y) - transform.position.y).normalized;
            //Debug.Log("nextNode (" + nextNode.x + ", " + nextNode.y + "), transformPosition = (" + transform.position.x + ", " + transform.position.y + ") | target direction = " + _targetDirection);
        }
        else
        {
            _targetDirection = Vector2.zero;
        }
    }

    protected void RotateTowardsTarget()
    {
        if (_targetDirection == Vector2.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        _rigidbody.SetRotation(rotation);
    }

    protected virtual void CalculatePath()
    {
        // Initialize variables
        Vector2 enemyPosition = GridMap.WorldToGridPositionVector(transform.position);
        Vector2 playerPosition = GridMap.WorldToGridPositionVector(_player.position);
        Node enemyGridPosition = GridMap.WorldToGridPosition(transform.position);
        Node playerGridPosition = GridMap.WorldToGridPosition(_player.position);

        // Find the path
        //Debug.Log("Sus FindPath... enemyGridPosition = (" + enemyGridPosition.x + ", " + enemyGridPosition.y + ") | playerGridPosition = " + playerGridPosition.x + ", " + playerGridPosition.y + ")");
        _currentPath = Pathfinding.FindPath(_grid, enemyGridPosition, playerGridPosition);
        //int pathLength = _currentPath.Count;
        //Debug.LogWarning("--------------");
        //string nodeList = "1 - path found (" + pathLength + ") = ";
        //foreach (var node in _currentPath)
        //{
        //    nodeList += "(" + node.x + ", " + node.y + "), ";
        //}
        //Debug.Log(nodeList);
        //// change the position of the node so that it corresponds to the enemy position, this prevents the enemy to go in unwanted direction first if it's a little far from the center of the tile
        //Debug.Log("currentNode = " + GridMap.GetNodeAtPosition((int)enemyPosition.x, (int)enemyPosition.y).x + ", " + GridMap.GetNodeAtPosition((int)enemyPosition.x, (int)enemyPosition.y).y + " | currentPath[1] = " + _currentPath[1].x + ", " + _currentPath[1].y + " | HasLineOfSight: " + GridMap.HasLineOfSight(GridMap.GetNodeAtPosition((int)enemyPosition.x, (int)enemyPosition.y), _currentPath[1], _grid));
        if (_currentPath.Count > 1)
        {
            if (GridMap.HasLineOfSight(GridMap.WorldToGridPosition((int)enemyPosition.x, (int)enemyPosition.y), _currentPath[1], _grid) && _currentPath.Count > 1)
            {
                Node enemyNode = new Node(enemyPosition.x, enemyPosition.y, true);
                _currentPath[0] = enemyNode;
            }

            Node playerNode = new Node(playerPosition.x, playerPosition.y, true);
            _currentPath[_currentPath.Count - 1] = playerNode;
        }

        //Debug.Log("currentPath[0] = " + _currentPath[0].x + ", " + _currentPath[0].y + " | enemyPosition");

        if (!IsDamaged && _currentPath != null && _currentPath.Count > 0)
        {
            //UpdateTargetDirection();
            //RotateTowardsTarget();
            //pathLength = _currentPath.Count;
            //nodeList = "2 - path found (" + pathLength + ") = ";
            //foreach (var node in _currentPath)
            //{
            //    nodeList += "(" + node.x + ", " + node.y + "), ";
            //}
            //Debug.Log(nodeList);
            //Debug.Break();
            FollowPath(_currentPath);
        }
    }

    protected virtual void FollowPath(List<Node> path)
    {
        if(path == null || path.Count == 0)
        {
            return;
        }

        // Get the next node in the path.
        Node nextNode = path[0];
        //Debug.Log("next node = " +  nextNode.xOnGrid + ", " + nextNode.yOnGrid);
        
        Vector2 targetPosition = GridMap.GridToWorldPosition(nextNode);

        Node nextNodeOnGrid = GridMap.WorldToGridPosition((int) nextNode.x, (int) nextNode.y);
        Node currentNode = GridMap.WorldToGridPosition(transform.position);
        //int x = (int)currentNode.x;
        //int y = (int)currentNode.y;

        // Obstacle avoidance for the A* algorithm
        //Debug.Log("currentNode = (" + currentNode.x + ", " + currentNode.y + ") | nextNode = (" + nextNode.x + ", " + nextNode.y + ") | HasOneAdjacentObstacle = " + HasOneSharedAdjacentObstacle(currentNode, nextNodeOnGrid) + ", IsDiagonal = " + GridMap.IsDiagonal(currentNode, nextNode) + ", Distance = " + (int)Vector2.Distance(transform.position, targetPosition));
        // If only one adjacent node is an obstacle, temporarily move horizontally or vertically.
        if (HasOneSharedAdjacentObstacle(currentNode, nextNodeOnGrid) && GridMap.IsDiagonal(currentNode, nextNodeOnGrid) /*&& (int)Vector2.Distance(transform.position, targetPosition) == 1*/)
        {
            
            // Move towards the target in either the horizontal or vertical direction.
            Dictionary<string, Node> sharedAdjacentNodes = GetSharedAdjacentNodes(currentNode, nextNodeOnGrid);
            if (sharedAdjacentNodes["horizontal"].walkable && !path.Contains(sharedAdjacentNodes["horizontal"]))
            {
                //Debug.Log("Inserted sharedAdjacentNodes[horizontal] (" + sharedAdjacentNodes["horizontal"].x + ", " + sharedAdjacentNodes["horizontal"].y + ")");

                path.Insert(0, sharedAdjacentNodes["horizontal"]);
                nextNode = path[0];
            }
            else if (sharedAdjacentNodes["vertical"].walkable && !path.Contains(sharedAdjacentNodes["vertical"]))
            {
                //Debug.Log("Inserted sharedAdjacentNodes[vertical] (" + sharedAdjacentNodes["vertical"].x + ", " + sharedAdjacentNodes["vertical"].y + ")");

                path.Insert(0, sharedAdjacentNodes["vertical"]);
                nextNode = path[0];

            }
            //int pathLength = path.Count;
            //string nodeList = "1 - path found (" + pathLength + ") = ";
            //foreach (var node in path)
            //{
            //    nodeList += "(" + node.x + ", " + node.y + "), ";
            //}
            //Debug.Log(nodeList);
        }

        // Calculate the direction to the next node.
        //Debug.Log("nextNode = " + nextNode.x + ", " + nextNode.y);
        UpdateTargetDirection(nextNode);
        RotateTowardsTarget();

        _rigidbody.velocity = _targetDirection * Speed;

        // If the enemy is close to the next node, remove it from the path.
        targetPosition = GridMap.GridToWorldPosition(nextNode);
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            _rigidbody.velocity = Vector2.zero;
            //Debug.Log("node (" + nextNode.x + ", " + nextNode.y + ") reached, removing node...");
            path.RemoveAt(0);
            //int pathLength = path.Count;
            //string nodeList = "currentNode = (" + currentNode.x + ", " + currentNode.y + ") | currentPath (" + pathLength + ") = ";
            //foreach (var node in path)
            //{
            //    nodeList += "(" + node.x + ", " + node.y + "), ";
            //}
            //Debug.Log(nodeList);
        }
    }

    protected IEnumerator RecalculatePathPeriodically()
    {
        while (true)
        {
            CalculatePath();
            yield return new WaitForSeconds(_pathRecalculationInterval);
        }
    }

     // This method checks if the next node has ONLY one adjacent vertical/horizontal node which is an obstacle
     protected bool HasOneSharedAdjacentObstacle(Node currentNode, Node nextNode)
     {
        int x = (int) currentNode.x;
        int y = (int) currentNode.y;

        // Count the number of adjacent vertical/horizontal nodes that are obstacles
        int obstacleCount = 0;

        // Calculate relative positions
        int xRel = Math.Sign(nextNode.x - currentNode.x);
        int yRel = Math.Sign(nextNode.y - currentNode.y);

        switch (xRel)
        {
            case -1:
                // Check node adjacent to the left of currentNode
                if (x > 0 && !_grid[x - 1, y].walkable)
                {
                    obstacleCount++;
                }
                break;
            case 1:
                // Check node adjacent to the right of currentNode
                if (x < _grid.GetLength(0) - 1 && !_grid[x + 1, y].walkable)
                {
                    obstacleCount++;
                }
                break;
        }

        switch (yRel)
        {
            case -1:
                // Check node adjacent to the bottom of currentNode
                if (y > 0 && !_grid[x, y - 1].walkable)
                {
                    obstacleCount++;
                }
                break;
            case 1:
                // Check node adjacent to the top of currentNode
                if (y < _grid.GetLength(1) - 1 && !_grid[x, y + 1].walkable)
                {
                    obstacleCount++;
                }
                break;
        }

        // Returns true only if obstacleCount is 1
        return obstacleCount == 1;
    }

    protected Dictionary<string, Node> GetSharedAdjacentNodes(Node currentNode, Node nextNode)
    {
        Dictionary<string, Node> sharedAdjacentNodes = new Dictionary<string, Node>();

        int x = (int)currentNode.x;
        int y = (int)currentNode.y;

        // Calculate relative positions
        int xRel = Math.Sign(nextNode.x - currentNode.x);
        int yRel = Math.Sign(nextNode.y - currentNode.y);

        switch (xRel)
        {
            case -1:
                // Check node adjacent to the left of currentNode
                sharedAdjacentNodes.Add("horizontal", _grid[x - 1, y]);
                break;
            case 1:
                // Check node adjacent to the right of currentNode
                sharedAdjacentNodes.Add("horizontal", _grid[x + 1, y]);
                break;
        }

        switch (yRel)
        {
            case -1:
                // Check node adjacent to the bottom of currentNode
                sharedAdjacentNodes.Add("vertical", _grid[x, y - 1]);
                break;
            case 1:
                // Check node adjacent to the top of currentNode
                sharedAdjacentNodes.Add("vertical", _grid[x, y + 1]);
                break;
        }

        return sharedAdjacentNodes;
    }
}

