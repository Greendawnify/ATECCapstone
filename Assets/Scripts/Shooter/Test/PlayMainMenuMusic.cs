using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMainMenuMusic : MonoBehaviour
{
    public AudioClip music;

    AudioSource source;
    MusicManager musicManager;
    // Start is called before the first frame update
    void Start()
    {
        musicManager = MusicManager.Instance;
        source = GetComponent<AudioSource>();

        if (musicManager.GetCanPlayMusic())
        {
            source.loop = true;
            source.clip = music;
            source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlay(bool play) {
        if (play)
        {
            //source.enabled = true;
            source.loop = true;
            source.clip = music;
            source.Play();
        }
        else {
            source.Stop();
            //source.enabled = false;
        }
    }
}
