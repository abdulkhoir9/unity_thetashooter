using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryWriter : MonoBehaviour
{
    [SerializeField]
    private float _typewriterDelay;

    [SerializeField]
    private TMP_Text _message;

    private string _messageText;

    [SerializeField]
    private GameObject _continueButton;

    [SerializeField]
    private string _nextScene;

    private void Start()
    {
        _messageText = _message.text;
        _message.text = "";
        _continueButton.SetActive(false);
        StartCoroutine("Typewriter");
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            StopCoroutine("Typewriter");
            _message.text = _messageText;
            _continueButton.SetActive(true);
        }
    }

    public void ContinueToNextScene()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        if(_nextScene == "MainMenu")
            AudioManager.instance.PlayMusic("MainMenu");
        
        SceneManager.LoadScene(_nextScene);
    }

    private IEnumerator Typewriter()
    {
        foreach(char c in _messageText)
        {
            _message.text += c;

            if (c == '\n')
                yield return new WaitForSeconds(_typewriterDelay * 10);
            else if (c == '.' || c == ',')
                yield return new WaitForSeconds(_typewriterDelay * 3);
            else
                yield return new WaitForSeconds(_typewriterDelay);
        }

        _continueButton.SetActive(true);
    }
}
