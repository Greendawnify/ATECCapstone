using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioClip standByMusic;
    public AudioClip gameMusic;
    public AudioClip heartBeat;
    public AudioMixerSnapshot unpaused;
    public AudioMixerSnapshot paused;
    public AudioSource source;
    public AudioSource heartbeatSource;

    bool playMusicSounds = true;
    bool isPaused = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        OptionData data = SaveScoreSystem.GetOptionData();

        if (data == null)
        {
            return;
        }
        else {
            playMusicSounds = data.playMusic;
        }
    }

    public void PlayStabByUI() {
        if (!playMusicSounds) {
            return;
        }

        source.Stop();
        source.clip = standByMusic;
        source.loop = true;
        source.Play();
    }

    public void PlayGameMusic() {
        if (!playMusicSounds) {
            return;
        }

        source.Stop();
        source.clip = gameMusic;
        source.loop = true;
        source.Play();
    }

    public void PlayHeartBeat() {
        if (!playMusicSounds)
            return;

        heartbeatSource.loop = true;
        heartbeatSource.clip = heartBeat;
        heartbeatSource.Play();
    }

    public void StopMusic() {
        Pause();

        source.Stop();

        heartbeatSource.Stop();
    }

    public void StopHeartbeat() {
        if (!playMusicSounds)
            return;

        heartbeatSource.Stop();
    }

    public void Pause() {
        if (!isPaused)
        {
            isPaused = true;
            paused.TransitionTo(0.1f);
        }
        else {
            isPaused = false;
            unpaused.TransitionTo(0.1f);
        }
    }



    public void SetCanPlayMusic(bool play) {
        playMusicSounds = play;

        if (!playMusicSounds)
            StopMusic();
    }

    public bool GetCanPlayMusic() {
        return playMusicSounds;
    }


}
