using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PathfindingDebug : MonoBehaviour
{
    private Camera _camera;

    [SerializeField]
    private Transform _player;

    [SerializeField]
    private Transform _enemy;

    public Tilemap _tilemap {  get; private set; }

    [SerializeField]
    private GameObject[] _gridLayoutArray;

    [SerializeField]
    private TMP_Text _gridSizeText;

    [SerializeField]
    private TMP_InputField _playerInputFieldX;

    [SerializeField]
    private TMP_InputField _playerInputFieldY;

    [SerializeField]
    private TMP_InputField _enemyInputFieldX;

    [SerializeField]
    private TMP_InputField _enemyInputFieldY;

    [SerializeField]
    private TMP_Dropdown _layoutDropdown;

    [SerializeField]
    private TMP_Dropdown _pathfindingAlgorithmDropdown;

    [SerializeField]
    private TMP_Text _resultText;

    [SerializeField]
    private GameObject _errorMessage;

    [SerializeField]
    private Button _calculateButton;
    
    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private float _coroutineWaitTime;

    private static LineRenderer _lineRenderer;
    private string _initialPathfindingAlgorithm;

    public static bool PositionIsValid;
    public static bool IsCalculatingPath;
    public static bool IsPaused;

    public static class DebugResult
    {
        private static List<Node> path;
        private static int pathLength;
        private static int nodesVisited;
        private static double computationTime;

        public static void SetResultValues(List<Node> p, int pl, int nv, double ct)
        {
            path = p;
            pathLength = pl;
            nodesVisited = nv;
            computationTime = ct;
        }

        public static string GetResult()
        {
            string pathString = "";
            string result;

            if (pathLength > 0)
                pathString += $"({ (int) path[0].x }, {(int)path[0].y })";

            for (int i = 1; i < pathLength; i++)
            {
                pathString += $", ({ (int) path[i].x }, { (int) path[i].y })";
            }

            result = $"Path found:\n{ pathString }\n\n" +
                $"Path length:\n{ pathLength } node(s)\n\n" +
                $"Number of nodes visited:\n{ nodesVisited } node(s)\n\n" +
                $"Computation time:\n{ computationTime } ms";

            return result;
        }
    }

    public static PathfindingDebug Instance;

    private void Awake()
    {
        PositionIsValid = true;
        IsCalculatingPath = false;
        IsPaused = false;
        Instance = this;
    }

    private void Start()
    {
        // Assign the main camera
        _camera = GetComponent<Camera>();

        // Assign default layout
        _tilemap = _gridLayoutArray[0].GetComponentInChildren<GridMap>().tileMap;

        // Create a LineRenderer object which we're going to use to visualize the path
        CreateLineRenderer();

        // Change camera size so that the grid fits inside the camera view
        UpdateCameraSize();

        // Show the grid size through the grid size text
        UpdateGridSizeText();

        // Initialize the positions input field according to the starting location of the player and enemy
        InitializeInputField();

        // Store the current setting for pathfinding algorithm. The pathfinding algorithm will be restored to its initial value after quitting the scene
        _initialPathfindingAlgorithm = PlayerPrefs.GetString("pathfindingAlgorithm");

        // Initialize theta* as the default pathfinding algorithm for the pathfinding debug
        PlayerPrefs.SetString("pathfindingAlgorithm", "theta*");
    }

    private void Update()
    {
        _pauseButton.interactable = IsCalculatingPath;
    }

    public void UpdatePosition()
    {
        int playerInputX = int.Parse(_playerInputFieldX.text);
        int playerInputY = int.Parse(_playerInputFieldY.text);
        int enemyInputX = int.Parse(_enemyInputFieldX.text);
        int enemyInputY = int.Parse(_enemyInputFieldY.text);

        Vector2 targetPlayerPosition = GridMap.GridToWorldPosition(new Node(playerInputX, playerInputY, true));
        Vector2 targetEnemyPosition = GridMap.GridToWorldPosition(new Node(enemyInputX, enemyInputY, true));

        Node targetPlayerNode = GridMap.WorldToGridPosition(targetPlayerPosition);
        Node targetEnemyNode = GridMap.WorldToGridPosition(targetEnemyPosition);

        if (targetPlayerNode == null || targetEnemyNode == null ||
            !targetPlayerNode.walkable || !targetEnemyNode.walkable)
        {
            // show error
            PositionIsValid = false;
            ShowErrorMessage();
            Debug.Log("Input position not valid. Input position is either out of bounds or is an obstacle.");
            return;
        }

        PositionIsValid = true;
        _player.position = GridMap.GridToWorldPosition(targetPlayerNode); 
        _enemy.position = GridMap.GridToWorldPosition(targetEnemyNode);
    }

    private void UpdateGridSizeText()
    {
        _gridSizeText.text = $"Grid size: { _tilemap.size.x } x { _tilemap.size.y } ";
    }

    private void InitializeInputField()
    {
        Vector2 playerPosition = GridMap.WorldToGridPositionVector(_player.position);
        Vector2 enemyPosition = GridMap.WorldToGridPositionVector(_enemy.position);

        _playerInputFieldX.text = ((int) playerPosition.x).ToString();
        _playerInputFieldY.text = ((int) playerPosition.y).ToString();
        _enemyInputFieldX.text = ((int) enemyPosition.x).ToString();
        _enemyInputFieldY.text = ((int) enemyPosition.y).ToString();
    }

    private void CreateLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.widthMultiplier = .05f;
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;
        _lineRenderer.sortingLayerName = "Enemy";
    }

    public static void DrawLine(List<Node> path)
    {
        _lineRenderer.positionCount = path.Count;

        int i = 0;
        foreach(Node node in path)
        {
            Vector2 nodePosition = GridMap.GridToWorldPosition(node);
            _lineRenderer.SetPosition(i++, nodePosition);
        }
    }

    public static void ClearAllTilesColor()
    {
        foreach (Vector3Int tilePosition in Instance._tilemap.cellBounds.allPositionsWithin)
        {
            Instance._tilemap.SetColor(tilePosition, Color.white);
        }
    }

    public static void ClearLine()
    {
        _lineRenderer.positionCount = 0;
    }

    private void UpdateCameraSize()
    {
        if(_tilemap.size.y > _tilemap.size.x || _tilemap.size.y == _tilemap.size.x)
            _camera.orthographicSize = _tilemap.size.y / 2;
        else
            _camera.orthographicSize = _tilemap.size.x / _camera.aspect / 2f;
    }

    private void RepositionCharacter()
    {
        Node playerNode = GridMap.WorldToGridPosition(_player.position);
        Node enemyNode = GridMap.WorldToGridPosition(_enemy.position);

        if (playerNode == null || !playerNode.walkable)
            playerNode = Pathfinding.FindNearestWalkableNode(GridMap.grid, _player.position);

        if (enemyNode == null || !enemyNode.walkable)
            enemyNode = Pathfinding.FindNearestWalkableNode(GridMap.grid, _enemy.position);

        _player.position = GridMap.GridToWorldPosition(playerNode);
        _enemy.position = GridMap.GridToWorldPosition(enemyNode);
    }

    public void BackToSettingsMenu()
    {
        PlayerPrefs.SetString("pathfindingAlgorithm", _initialPathfindingAlgorithm);

        AudioManager.instance.PlaySFX("ButtonBack");
        Time.timeScale = 1f;

        SceneManager.LoadScene("SettingsMenu");
    }

    public void SwitchGridLayout()
    {
        int value = _layoutDropdown.value;

        // Disable grid
        foreach(GameObject grid in _gridLayoutArray)
        {
            grid.SetActive(false);
        }

        // Set the selected grid as active
        GameObject selectedGrid = _gridLayoutArray[value];
        selectedGrid.SetActive(true);
        CancelPathfindingCalculation();
        _calculateButton.GetComponentInChildren<TMP_Text>().text = "Calculate Path";
        _pauseButton.GetComponentInChildren<TMP_Text>().text = "Pause Calculation";
        _tilemap = selectedGrid.GetComponentInChildren<GridMap>().tileMap;
        GridMap.InitializeGrid(_tilemap);
        UpdateCameraSize();
        UpdateGridSizeText();
        RepositionCharacter();

        InitializeInputField();
    }

    public void SwitchPathfindingAlgorithm()
    {
        int value = _pathfindingAlgorithmDropdown.value;

        switch (value)
        {
            case 0:
                PlayerPrefs.SetString("pathfindingAlgorithm", "theta*");
                break;
            case 1:
                PlayerPrefs.SetString("pathfindingAlgorithm", "theta*-raycast");
                break;
            case 2:
                PlayerPrefs.SetString("pathfindingAlgorithm", "a*");
                break;
        }
    }

    public void ShowErrorMessage()
    {
        bool isActive = _errorMessage.activeInHierarchy;

        if (isActive)
            AudioManager.instance.PlaySFX("ButtonBack");
        else
            AudioManager.instance.PlaySFX("ButtonClick");

        _errorMessage.SetActive(!isActive);
    }

    private void ShowResults()
    {
        _resultText.text = DebugResult.GetResult();
    }

    public void PausePathfindingCalculation()
    {
        if (IsPaused)
        {
            AudioManager.instance.PlaySFX("ButtonBack");
            CancelPathfindingCalculation();
            _calculateButton.GetComponentInChildren<TMP_Text>().text = "Calculate Path";
            _pauseButton.GetComponentInChildren<TMP_Text>().text = "Pause Calculation";
        }
        else
        {
            AudioManager.instance.PlaySFX("ButtonClick");
            IsPaused = true;
            Time.timeScale = 0f;
            _calculateButton.GetComponentInChildren<TMP_Text>().text = "Resume Calculation";
            _pauseButton.GetComponentInChildren<TMP_Text>().text = "Cancel Calculation";
        }
    }

    public static void ResumePathfindingCalculation()
    {
        Time.timeScale = 1.0f;
        IsPaused = false;
        Instance._calculateButton.GetComponentInChildren<TMP_Text>().text = "Calculate Path";
        Instance._pauseButton.GetComponentInChildren<TMP_Text>().text = "Pause Calculation";
    }

    public static void CancelPathfindingCalculation()
    {
        IsPaused = false;
        IsCalculatingPath = false;
        FrameRateStats.ToggleUpdate(false);
        ClearAllTilesColor();
        ClearLine();
        StopPathfindingCoroutine();
        Time.timeScale = 1f;
    }

    public static void StopPathfindingCoroutine()
    {
        Instance.StopAllCoroutines();
    }

    public static void CallPathfindingCoroutine(Node[,] grid, Node startNode, Node targetNode)
    {
        ClearLine();

        Instance.StartCoroutine(Instance.FindPathDebugCoroutine(grid, startNode, targetNode));
    }

    private IEnumerator FindPathDebugCoroutine(Node[,] grid, Node startNode, Node targetNode)
    {
        Vector2 startPosition = GridMap.GridToWorldPosition(startNode);
        Vector2 targetPosition = GridMap.GridToWorldPosition(targetNode);

        _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(startPosition.x), Mathf.FloorToInt(startPosition.y), 0), Color.green);
        _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(targetPosition.x), Mathf.FloorToInt(targetPosition.y), 0), Color.red);

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

            Vector2 currentPosition = GridMap.GridToWorldPosition(currentNode);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode != startNode && currentNode != targetNode)
            {
                yield return new WaitForSeconds(_coroutineWaitTime);
                _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y), 0), new Color(1f, .6f, .6f));
            }

            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                RetracePathDebug(startNode, currentNode);

                yield break;
            }

            foreach (Node neighbor in Pathfinding.GetNeighbors(currentNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + Pathfinding.GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = Pathfinding.GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    // FOR THETA*
                    if (PlayerPrefs.GetString("pathfindingAlgorithm") != "a")
                    {
                        if(currentNode.parent != null)
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
                    {
                        currentPosition = GridMap.GridToWorldPosition(neighbor);

                        openSet.Add(neighbor);
                        if (neighbor != targetNode)
                            _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y), 0), new Color(1f, 1f, .65f));
                    }
                }
                yield return new WaitForSeconds(_coroutineWaitTime);
            }
        }

        yield return null; // No path found.
    }

    private List<Node> RetracePathDebug(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        Vector2 currentPosition;

        while (currentNode != null && currentNode.parent != null &&
            !(currentNode.x == startNode.x && currentNode.y == startNode.y))
        {
            if (!path.Contains(currentNode))
            {
                currentPosition = GridMap.GridToWorldPosition(currentNode);
                path.Add(currentNode);
                _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y), 0), Color.black);
                currentNode = currentNode.parent;
            }
            else
            {
                break;
            }
        }

        if (currentNode.x == startNode.x && currentNode.y == startNode.y)
        {
            currentPosition = GridMap.GridToWorldPosition(startNode);
            path.Add(startNode);
            _tilemap.SetColor(new Vector3Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y), 0), Color.black);
        }

        path.Reverse();

        // Draw line
        DrawLine(path);
        ShowResults();

        IsCalculatingPath = false;
        FrameRateStats.ToggleUpdate(false);


        return path;
    }
}
