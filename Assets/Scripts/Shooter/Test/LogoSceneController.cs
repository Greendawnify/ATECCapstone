using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoSceneController : MonoBehaviour
{
    public AudioClip logo, standby;
    public float logoSongWait;
    public string scenename;

    AudioSource source;
    AsyncOperation operation;
    AdsController adScript;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        adScript = GetComponent<AdsController>();
        adScript.CallBannerAd();
        StartCoroutine(LogoSongWait());
        StartCoroutine(LoadAsync());
    }

    public void Proceed() {
        operation.allowSceneActivation = true;
    }

    IEnumerator LogoSongWait() { 
        yield return new WaitForSeconds(logoSongWait);
        source.clip = logo;
        float len = logo.length;
        source.Play();
        StartCoroutine(PlayStandBy(len));
    }

    IEnumerator PlayStandBy(float len) {
        yield return new WaitForSeconds(len);
        source.Stop();
        source.loop = true;
        source.clip = standby;
        source.Play();
    }

    IEnumerator LoadAsync() {
        operation = SceneManager.LoadSceneAsync(scenename);
        operation.allowSceneActivation = false;
        yield return null;

    }
}
