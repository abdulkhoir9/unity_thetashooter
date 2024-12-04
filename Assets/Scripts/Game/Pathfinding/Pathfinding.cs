using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding
{
    public static List<Node> FindPath(Node[,] grid, Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    // FOR THETA*
                    if (PlayerPrefs.GetString("pathfindingAlgorithm") != "a")
                    {
                        if (currentNode.parent != null)
                        {
                            bool hasLineOfSight = false;

                            switch (PlayerPrefs.GetString("pathfindingAlgorithm"))
                            {
                                case "theta*":
                                    hasLineOfSight = GridMap.HasLineOfSight(currentNode.parent, neighbor, grid);
                                    break;
                                case "theta*-raycast":
                                    hasLineOfSight = GridMap.HasLineOfSight(GridMap.GridToWorldPosition(currentNode.parent), GridMap.GridToWorldPosition(neighbor));
                                    break;
                            }

                            // Check for line-of-sight between parent and neighbor
                            if (hasLineOfSight && currentNode != startNode)
                                neighbor.parent = currentNode.parent;
                        }
                    }

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found.
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != null && currentNode.parent != null &&
            !(currentNode.x == startNode.x && currentNode.y == startNode.y))
        {
            if (!path.Contains(currentNode))
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            else
            {
                break;
            }
        }

        if (currentNode.x == startNode.x && currentNode.y == startNode.y)
        {
            path.Add(startNode);
        }
        
        path.Reverse();
        return path;
    }

    public static List<Node> GetNeighbors(Node node, Node[,] grid)
    {
        List<Node> neighbors = new List<Node>();
        int x = (int) node.x;
        int y = (int) node.y;
        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);

        //Debug.Log(x + ", " + y + " | gridWidth = " + gridWidth + ", gridHeight = " + gridHeight);
        // Check up
        if (y < gridHeight - 1)
            neighbors.Add(grid[x, y + 1]);

        // Check down
        if (y > 0)
            neighbors.Add(grid[x, y - 1]);

        // Check left
        if (x > 0) 
            neighbors.Add(grid[x - 1, y]);

        // Check right
        if (x < gridWidth - 1)
            neighbors.Add(grid[x + 1, y]);

        // Check top-left diagonal
        if (x > 0 && y < gridHeight - 1)
            if(grid[x - 1, y].walkable || grid[x, y + 1].walkable)
                neighbors.Add(grid[x - 1, y + 1]);

        // Check top-right diagonal
        if (x < gridWidth - 1 && y < gridHeight - 1)
            if (grid[x + 1, y].walkable || grid[x, y + 1].walkable)
                neighbors.Add(grid[x + 1, y + 1]);

        // Check bottom-left diagonal
        if (x > 0 && y > 0)
            if (grid[x - 1, y].walkable || grid[x, y - 1].walkable)
                neighbors.Add(grid[x - 1, y - 1]);

        // Check bottom-right diagonal
        if (x < gridWidth - 1 && y > 0)
            if (grid[x + 1, y].walkable || grid[x, y - 1].walkable)
                neighbors.Add(grid[x + 1, y - 1]);
        

        //string neighborList = "neighbors = ";
        //foreach (var neighbor in neighbors)
        //{
        //    neighborList += "(" + neighbor.x + ", " + neighbor.y + "), ";
        //}
        //Debug.Log(neighborList);

        return neighbors;
    }

    public static int GetDistance(Node a, Node b)
    {
        int distanceX = (int) Mathf.Abs(a.x - b.x);
        int distanceY = (int) Mathf.Abs(a.y - b.y);

        // The "cost" of moving horizontally and vertically is typically 1, but you can adjust it if needed.
        int movementCost = 1;

        return (distanceX + distanceY) * movementCost;
    }

    public static List<Node> FindPathDebug(Node[,] grid, Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();

        switch (PlayerPrefs.GetString("pathfindingAlgorithm"))
        {
            case "a*":
                path = FindPathDebugAStar(grid, startNode, targetNode);
                break;
            case "theta*":
                path = FindPathDebugThetaStar(grid, startNode, targetNode);
                break;
            case "theta*-raycast":
                path = FindPathDebugThetaStarRaycast(grid, startNode, targetNode);
                break;
        }

        return path;
    }

    private static List<Node> FindPathDebugAStar(Node[,] grid, Node startNode, Node targetNode)
    {
        if (!PathfindingDebug.PositionIsValid)
            return null;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                List<Node> path = RetracePath(startNode, currentNode);

                stopwatch.Stop();

                PathfindingDebug.DebugResult.SetResultValues(path, path.Count, closedSet.Count, stopwatch.Elapsed.TotalMilliseconds);

                PathfindingDebug.CallPathfindingCoroutine(grid, startNode, targetNode);

                return path;
            }

            foreach (Node neighbor in GetNeighbors(currentNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // No path found.
    }

    private static List<Node> FindPathDebugThetaStar(Node[,] grid, Node startNode, Node targetNode)
    {
        if (!PathfindingDebug.PositionIsValid)
            return null;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                List<Node> path = RetracePath(startNode, currentNode);

                stopwatch.Stop();

                PathfindingDebug.DebugResult.SetResultValues(path, path.Count, closedSet.Count, stopwatch.Elapsed.TotalMilliseconds);

                PathfindingDebug.CallPathfindingCoroutine(grid, startNode, targetNode);

                return path;
            }

            foreach (Node neighbor in GetNeighbors(currentNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);

                    if (currentNode.parent != null && GridMap.HasLineOfSight(currentNode.parent, neighbor, grid) && currentNode != startNode)
                        neighbor.parent = currentNode.parent;
                    else 
                        neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found.
    }

    private static List<Node> FindPathDebugThetaStarRaycast(Node[,] grid, Node startNode, Node targetNode)
    {
        if (!PathfindingDebug.PositionIsValid)
            return null;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                List<Node> path = RetracePath(startNode, currentNode);

                stopwatch.Stop();

                PathfindingDebug.DebugResult.SetResultValues(path, path.Count, closedSet.Count, stopwatch.Elapsed.TotalMilliseconds);

                PathfindingDebug.CallPathfindingCoroutine(grid, startNode, targetNode);

                return path;
            }

            foreach (Node neighbor in GetNeighbors(currentNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (currentNode.parent != null && GridMap.HasLineOfSight(GridMap.GridToWorldPosition(currentNode.parent), GridMap.GridToWorldPosition(neighbor)) && currentNode != startNode)
                        neighbor.parent = currentNode.parent;
                    else
                        neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // No path found.
    }

    public static Node FindNearestWalkableNode(Node[,] grid, Vector3 startPosition)
    {
        // Move position into the grid if it's out of bounds
        while (startPosition.x < GridMap.minX)
            startPosition.x++;

        while (startPosition.x > GridMap.minX + grid.GetLength(0))
            startPosition.x--;

        while (startPosition.y < GridMap.minY)
            startPosition.y++;

        while (startPosition.y > GridMap.minY + grid.GetLength(1))
            startPosition.y--;

        Node startNode = GridMap.WorldToGridPosition(startPosition);

        if (startNode.walkable)
            return startNode;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.walkable)
                return currentNode;

            foreach (Node neighbor in GetNeighbors(currentNode, grid))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, startNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return startNode; // No path found.
    }
}
