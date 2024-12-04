using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public Toggle _antiAliasingToggle, _vignetteToggle;
    public Volume _volume;
    public TMP_Dropdown _pathfindingAlgorithmDropdown;

    // Default settings
    public const float _defaultMusicVolume = 1f;
    public const float _defaultSfxVolume = 1f;
    public const string _defaultPathfindingAlgorithm = "theta*";
    public const int _defaultAntiAliasingSettings = 1;
    public const int _defaultVignetteSettings = 1;

    private void Start()
    {
        // Adjust music volume slider
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            AudioManager.instance.SetMusicVolume(musicVolume);
            _musicSlider.value = musicVolume;
        }
        else
        {
            AudioManager.instance.SetMusicVolume(_musicSlider.value);
        }


        // Adjust SFX volume slider
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            AudioManager.instance.SetSfxVolume(sfxVolume);
            _sfxSlider.value = sfxVolume;
        }
        else
        {
            AudioManager.instance.SetMusicVolume(_musicSlider.value);
        }

        // Adjust pathfinding algorithm dropdown
        if (PlayerPrefs.HasKey("pathfindingAlgorithm"))
        {
            string pathfindingAlgorithm = PlayerPrefs.GetString("pathfindingAlgorithm");
            int pathfindingDropdownValue = _pathfindingAlgorithmDropdown.value;

            if (pathfindingAlgorithm == "theta*")
                _pathfindingAlgorithmDropdown.value = 0;
            else
                _pathfindingAlgorithmDropdown.value = 1;

            if (pathfindingDropdownValue != _pathfindingAlgorithmDropdown.value)
                SetPathfindingAlgorithm();
        }
        else
        {
            PlayerPrefs.SetString("pathfindingAlgorithm", "theta*");
        }

        // Adjust anti aliasing toggle
        if (PlayerPrefs.HasKey("antiAliasing"))
        {
            int antiAliasing = PlayerPrefs.GetInt("antiAliasing");
            bool antiAliasingToggle = _antiAliasingToggle.isOn;
            ToggleAntiAliasingInCamera();

            if (antiAliasing == 1)
                _antiAliasingToggle.isOn = true;
            else
                _antiAliasingToggle.isOn = false;

            if (antiAliasingToggle != _antiAliasingToggle.isOn)
                ToggleAntiAliasing();
        }
        else
        {
            PlayerPrefs.SetInt("antiAliasing", 1);
        }

        //Adjust vignette toggle
        if (PlayerPrefs.HasKey("vignette"))
        {
            int vignette = PlayerPrefs.GetInt("vignette");
            bool vignetteToggle = _vignetteToggle.isOn;
            ToggleVignetteInVolume();

            if (vignette == 1)
                _vignetteToggle.isOn = true;
            else
                _vignetteToggle.isOn = false;
            
            if(vignetteToggle != _vignetteToggle.isOn)
                ToggleVignette();
        }
        else
        {
            PlayerPrefs.SetInt("vignette", 1);
        }
    }

    public void MusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", _musicSlider.value);
        AudioManager.instance.SetMusicVolume(_musicSlider.value);
    }

    public void SfxVolume()
    {
        PlayerPrefs.SetFloat("sfxVolume", _sfxSlider.value);
        AudioManager.instance.SetSfxVolume(_sfxSlider.value);
    }

    public void SetPathfindingAlgorithm()
    {
        string pathfindingAlgorithm = PlayerPrefs.GetString("pathfindingAlgorithm");

        if (pathfindingAlgorithm == "theta*")
        {
            PlayerPrefs.SetString("pathfindingAlgorithm", "a*");
            pathfindingAlgorithm = PlayerPrefs.GetString("pathfindingAlgorithm");
        }
        else
        {
            PlayerPrefs.SetString("pathfindingAlgorithm", "theta*");
            pathfindingAlgorithm = PlayerPrefs.GetString("pathfindingAlgorithm");
        }
    }

    public void ToggleAntiAliasing()
    {
        int antiAliasing = PlayerPrefs.GetInt("antiAliasing");
        if (antiAliasing == 1)
        {
            PlayerPrefs.SetInt("antiAliasing", 0);
            antiAliasing = PlayerPrefs.GetInt("antiAliasing");
        }
        else
        {
            PlayerPrefs.SetInt("antiAliasing", 1);
            antiAliasing = PlayerPrefs.GetInt("antiAliasing");
        }

        ToggleAntiAliasingInCamera();
    }

    public void ToggleVignette()
    {
        int vignette = PlayerPrefs.GetInt("vignette");
        if (vignette == 1)
        {
            PlayerPrefs.SetInt("vignette", 0);
            vignette = PlayerPrefs.GetInt("vignette");
        }
        else
        {
            PlayerPrefs.SetInt("vignette", 1);
            vignette = PlayerPrefs.GetInt("vignette");
        }

        ToggleVignetteInVolume();
    }

    private void ToggleAntiAliasingInCamera()
    {
        int antiAliasing = PlayerPrefs.GetInt("antiAliasing");

        if (antiAliasing == 0)
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.None;
        else
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.TemporalAntiAliasing;
    }

    private void ToggleVignetteInVolume()
    {
        if (_volume != null)
        {
            int vignette = PlayerPrefs.GetInt("vignette");
            Vignette v;
            _volume.profile.TryGet(out v);
            v.active = (vignette == 1);
        }
    }

    public void ResetSettings()
    {
        if(PlayerPrefs.GetFloat("musicVolume") != _defaultMusicVolume)
        {
            PlayerPrefs.SetFloat("musicVolume", _defaultMusicVolume);
            AudioManager.instance.SetMusicVolume(_defaultMusicVolume);
            _musicSlider.value = _defaultMusicVolume;
        }

        if (PlayerPrefs.GetFloat("sfxVolume") != _defaultSfxVolume)
        {
            PlayerPrefs.SetFloat("sfxVolume", _defaultSfxVolume);
            AudioManager.instance.SetSfxVolume(_defaultSfxVolume);
            _sfxSlider.value = _defaultSfxVolume;
        }

        if (PlayerPrefs.GetString("pathfindingAlgorithm") != _defaultPathfindingAlgorithm)
        {
            _pathfindingAlgorithmDropdown.value = 0;
        }

        if (PlayerPrefs.GetInt("antiAliasing") != _defaultAntiAliasingSettings)
        {
            _antiAliasingToggle.isOn = !_antiAliasingToggle.isOn;
        }

        if (PlayerPrefs.GetInt("vignette") != _defaultVignetteSettings)
        {
            _vignetteToggle.isOn = !_vignetteToggle.isOn;
        }
    }
}
