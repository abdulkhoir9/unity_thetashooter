using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private GameObject _levelCompletedUI;

    [SerializeField]
    private GameObject _gameOverUI;

    [SerializeField]
    private GameObject _fieldOfView;

    [SerializeField]
    private string _nextScene;

    [SerializeField]
    private GameObject _healthBoxPrefab;

    private static GameObject _healthBoxPrefab2;
    public static int _score;
    public static int _spawnCount;

    [SerializeField]
    private int _scoreToComplete;

    public const int _scoreToSpawn = 10;

    public static bool _isStopped;

    private static Vector2 _healthBoxSpawnPosition;

    private void Awake()
    {
        _healthBoxPrefab2 = _healthBoxPrefab;
        _score = 0;
        _spawnCount = 0;
    }

    public static void AddScore(int score)
    {
        _score += score;
    }

    public static void SetHealthBoxSpawnPosition(Vector2 position)
    {
        _healthBoxSpawnPosition = position;
    }

    public static void SpawnHealthBox()
    {
        GameObject healthBox = Instantiate(_healthBoxPrefab2, _healthBoxSpawnPosition, Quaternion.identity);
        AudioManager.instance.PlaySFX("HealthBoxDrop");

        _spawnCount++;
    }

    // Complete the level
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("triggered\nplayermovement: " + collision.GetComponent<PlayerMovement>() + ", score: " + _score);
        if(collision.GetComponent<PlayerMovement>() && _score >= _scoreToComplete)
        {
            Time.timeScale = 0f;
            _isStopped = true;
            _levelCompletedUI.SetActive(true);
            AudioManager.instance.PlayMusic("Win");
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        _isStopped = true;
        _gameOverUI.SetActive(true);
        _fieldOfView.SetActive(false);
        AudioManager.instance.PlayMusic("GameOver");
    }

    public void RestartLevel()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        AudioManager.instance.PlayMusic("LevelBGM");
        Time.timeScale = 1f;
        _isStopped = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextScene()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        AudioManager.instance.PlayMusic("LevelBGM");
        Time.timeScale = 1f;
        _isStopped = false;
        SceneManager.LoadScene(_nextScene);
    }

    public void QuitToMainMenu()
    {
        AudioManager.instance.PlaySFX("ButtonBack");
        AudioManager.instance.PlayMusic("MainMenu");
        Time.timeScale = 1f;
        _isStopped = false;
        SceneManager.LoadScene("MainMenu");
    }
}
