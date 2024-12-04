using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string _name;
    public AudioClip _clip;

    [Range(0f, 1f)]
    public float _volume;

    public bool _loop;
}
