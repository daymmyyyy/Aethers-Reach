using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip backgroundMusic;
    void Start()
    {
        AudioManager.Instance.PlayMusic(backgroundMusic);

    }
}
