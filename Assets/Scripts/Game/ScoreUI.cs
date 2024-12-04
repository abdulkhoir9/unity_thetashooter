using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private static TMP_Text _scoreText;

    private void Awake()
    {
        _scoreText = GetComponent<TMP_Text>();
    }

    public static void UpdateScoreText()
    {
        _scoreText.text = $"Score: {LevelController._score} ";
    }
}
