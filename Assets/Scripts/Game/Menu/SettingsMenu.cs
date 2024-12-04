using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _resetWarning;

    public void BackToMainMenu()
    {
        AudioManager.instance.PlaySFX("ButtonBack");
        SceneManager.LoadScene("MainMenu");
    }

    public void EnterDebugMode()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        SceneManager.LoadScene("PathfindingDebug");
    }

    public void ShowResetWarning()
    {
        bool isActive = _resetWarning.activeInHierarchy;

        if (isActive)
            AudioManager.instance.PlaySFX("ButtonBack");
        else
            AudioManager.instance.PlaySFX("ButtonClick");

        _resetWarning.SetActive(!isActive);
    }
}
