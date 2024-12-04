using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }    
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Choose which music to play 
        if(SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "SettingsMenu" || SceneManager.GetActiveScene().name == "PathfindingDebug")
            PlayMusic("MainMenu");
        else
            PlayMusic("LevelBGM");

        // Set the volume level
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            AudioManager.instance.SetMusicVolume(musicVolume);
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            AudioManager.instance.SetSfxVolume(sfxVolume);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x._name == name);

        if (s == null)
        {
            Debug.Log("Couldn't find the specified sound");
        }
        else
        {
            musicSource.clip = s._clip;
            musicSource.loop = s._loop;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x._name == name);

        if (s == null)
        {
            Debug.Log("Couldn't find the specified sound");
        }
        else
        {
            sfxSource.PlayOneShot(s._clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
