using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap : MonoBehaviour
{
    public Tilemap tileMap { get; private set; }    // The tilemap which will be represented as a grid
    public static Node[,] grid;        // The grid used for the pathfinding, in the form of Nodes stored in 2d array

    public static int minX, minY;      // Minimum point of the tilemap, on the x and y axis of the game world (used to transform node position between grid and world position)

    private void Awake()
    {
        // Get the tilemap component
        tileMap = GetComponent<Tilemap>();

        // Initialize the grid when the game starts
        InitializeGrid(tileMap);
    }

    // The InitializeGrid() method assigns values for each node inside the grid
    public static void InitializeGrid(Tilemap tilemap)
    {
        int gridWidth = tilemap.size.x;     // The width of the tilemap
        int gridHeight = tilemap.size.y;    // The height of the tilemap
        minX = tilemap.cellBounds.xMin; 
        minY = tilemap.cellBounds.yMin;

        Vector2 cellSize = tilemap.cellSize;    // Get the cell size to center the nodes

        Node[,] gridMap = new Node[gridWidth, gridHeight]; // Assign a new 2d Node array with size matching the width and height of the tilemap

        // The following for loop assigns the position for each nodes
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int xOnWorld = minX + x;
                int yOnWorld = minY + y;
                Vector3Int cellPosition = new Vector3Int(xOnWorld, yOnWorld, 0);
                tilemap.RemoveTileFlags(cellPosition, TileFlags.LockColor);
                TileBase tile = tilemap.GetTile(cellPosition);

                bool isWalkable = (tile != null);
                // to center the tiles
                float centerX = x * cellSize.x + cellSize.x / 2f;
                float centerY = y * cellSize.y + cellSize.y / 2f;

                gridMap[x, y] = new Node(centerX, centerY, isWalkable);
            }
        }

        grid = gridMap;
    }

    // Get node at position x, y on the grid
    public static Node WorldToGridPosition(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        else
        {
            return null;
        }
    }

    public static Node WorldToGridPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x) - minX;
        int y = Mathf.FloorToInt(worldPosition.y) - minY;

        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        else
        {
            return null;
        }
    }

    public static Vector2 WorldToGridPositionVector(Vector2 worldPosition)
    {
        float x = worldPosition.x - minX;
        float y = worldPosition.y - minY;

        return new Vector2(x, y);
    }

    public static Vector2 GridToWorldPosition(Node node)
    {
        Vector2 worldPosition = new Vector2(node.x + minX, node.y + minY);

        return worldPosition;
    }

    public static bool IsDiagonal(Node firstNode, Node secondNode)
    {
        return ((firstNode.x != secondNode.x) && (firstNode.y != secondNode.y));
    }

    public static bool HasLineOfSight(Node startNode, Node endNode, Node[,] grid)
    {
        // Checks line of sight using Bresenham's Line Algorithm 
        float startX = startNode.x;
        float startY = startNode.y;
        float endX = endNode.x;
        float endY = endNode.y;

        float deltaX = Mathf.Abs(endX - startX);
        float deltaY = Mathf.Abs(endY - startY);
        int signX = startX < endX ? 1 : -1;
        int signY = startY < endY ? 1 : -1;

        float error = deltaX - deltaY;

        while (startX != endX || startY != endY)
        {
            float error2 = error * 2;

            if (error2 > -deltaY)
            {
                error -= deltaY;
                startX += signX;
            }

            if (error2 < deltaX)
            {
                error += deltaX;
                startY += signY;
            }

            if (!grid[(int)startX, (int)startY].walkable)
            {
                return false; // Obstacle found, no line-of-sight
            }
        }

        return true; // Line-of-sight is clear
    }

    public static bool HasLineOfSight(Vector2 position1, Vector2 position2)
    {
        // Checks line of sight using Raycast
        RaycastHit2D hit = Physics2D.Raycast(position1, position2 - position1, Vector2.Distance(position1, position2), LayerMask.GetMask("Walls"));

        return hit.collider == null;
    }
}

// The Node class stores the informations of a node
public class Node
{
    public float x, y;              // position on the grid, x and y points to the center of the corresponding tile
    public bool walkable;           // determines whether this node is walkable or not. true means walkable. walkable node represents floor tile, while non-walkable node represents obstacle
    public int gCost;               // cost from start node to this node
    public int hCost;               // heuristic cost from this node to the target node
    public Node parent;             // parent node of the current node

    // the total cost (fcost) equals the cost from start node to this node (gcost) plus the estimated cost from this node to the target node (hcost)
    public int fCost
    {
        get { return gCost + hCost; }
    }

    // Node constructor
    public Node(float x, float y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }
}
