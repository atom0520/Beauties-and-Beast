using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    public AudioSource bgmAudioSrc;

    [SerializeField]
    public AudioSource callInGameMenuAudioSrc;

    [SerializeField]
    public AudioSource inGameMenuToggleAudioSrc;

    [SerializeField]
    public AudioSource defeatAudioSrc;

    [SerializeField]
    public AudioSource victoryAudioSrc;

    [SerializeField]
    public AudioSource startFightingAudioSrc;
}
