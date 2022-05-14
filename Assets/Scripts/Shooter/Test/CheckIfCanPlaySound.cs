using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfCanPlaySound : MonoBehaviour
{

    AudioManager audioManager;
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
        source = GetComponent<AudioSource>();
        CheckIfCanPlay();
    }

    private void OnEnable()
    {
        CheckIfCanPlay();
    }

    void CheckIfCanPlay() {
        if (!audioManager.GetIfCanPlay() && audioManager != null)
            source.enabled = false;
        else
            source.enabled = true;
    }

}
