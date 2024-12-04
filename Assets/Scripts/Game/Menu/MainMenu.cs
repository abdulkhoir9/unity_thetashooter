using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        AudioManager.instance.PlayMusic("LevelBGM");
        SceneManager.LoadScene("Prologue");
    }

    public void OpenSettings()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        SceneManager.LoadScene("SettingsMenu");
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        Application.Quit();
    }
}
