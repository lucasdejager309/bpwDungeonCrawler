using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SoundEffect {
    public string name;
    public AudioClip audioClip;
}

public class AudioManager : Singleton<AudioManager>
{
    public Text subtitles;

    [SerializeField] SoundEffect[] soundEffects;
    private AudioSource audioSource;

    void Awake() {
        Instance = this;
    }

    public void PlaySound(string name) {
        if (audioSource == null) {
            GameObject source = new GameObject("audioSource");
            audioSource = source.AddComponent<AudioSource>();
        }
        foreach (SoundEffect sound in soundEffects) {
            if (sound.name == name) {
                audioSource.PlayOneShot(sound.audioClip);
            }
        }
    }
}
