using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;

    }
    public AudioSource sound;
    public AudioSource music;
    [Header("GameSounds")]
    [SerializeField] AudioClip swapPieces;
    [SerializeField] AudioClip matchPieces;
    [SerializeField] AudioClip completedLevel;

    [Header("Game Music")]
    [SerializeField] AudioClip gameMusic;

    [Header("UI Sounds")]
    [SerializeField] AudioClip buttonClick;

    public void SetSoundVolume(float volume)
    {
        sound.volume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        music.volume = volume;
    }
    public void PlaySwapSound()
    {
        sound.PlayOneShot(swapPieces);
    }
    public void PlayMatchSound()
    {
        sound.PlayOneShot(matchPieces);
    }
    public void PlayButtonClickSound()
    {
        sound.PlayOneShot(buttonClick);
    }
    public void PlayCompletedLevelSound()
    {
        sound.PlayOneShot(completedLevel);
    }


}
