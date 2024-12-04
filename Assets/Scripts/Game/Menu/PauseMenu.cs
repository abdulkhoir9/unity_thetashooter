using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _pauseMenu;

    [SerializeField]
    private GameObject _pauseMenuContent;

    [SerializeField]
    private GameObject _settingsMenu;

    [SerializeField]
    private GameObject _resetWarning;

    [SerializeField]
    private GameObject _healthBar;

    [SerializeField]
    private GameObject _bulletCountText;

    [SerializeField]
    private GameObject _scoreText;

    [SerializeField]
    private GameObject _globalVolume;

    [SerializeField]
    private float _toggledVignetteIntensity;

    public static bool _isPaused;
    private bool _isOnSettings;
    private float _defaultVignetteIntensity;

    private void Awake()
    {
        Volume volume = _globalVolume.GetComponent<Volume>();

        Vignette vignette;
        volume.profile.TryGet(out vignette);

        _defaultVignetteIntensity = vignette.intensity.value;
    }

    private void Start()
    {
        _pauseMenuContent.SetActive(true);
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isPaused)
            {
                if(_isOnSettings)
                {
                    CloseSettings();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        AudioManager.instance.PlaySFX("ButtonBack");
        _isPaused = true;
        HideUI();
        ToggleVignetteIntensity();
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        _isPaused = false;
        HideUI();
        ToggleVignetteIntensity();
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        _isOnSettings = true;
        _pauseMenuContent.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioManager.instance.PlaySFX("ButtonBack");
        _isOnSettings = false;
        _settingsMenu.SetActive(false);
        _pauseMenuContent.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        AudioManager.instance.PlaySFX("ButtonBack");
        AudioManager.instance.PlayMusic("MainMenu");
        _isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

    private void HideUI()
    {
        _healthBar.SetActive(!_isPaused);
        _bulletCountText.SetActive(!_isPaused);
        _scoreText.SetActive(!_isPaused);
    }

    private void ToggleVignetteIntensity()
    {
        Volume volume = _globalVolume.GetComponent<Volume>();

        float intensity = _isPaused ? _toggledVignetteIntensity : _defaultVignetteIntensity;

        Vignette vignette;
        volume.profile.TryGet<Vignette>(out vignette);

        vignette.intensity.value = intensity;
    }
}
