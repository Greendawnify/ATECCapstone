using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public GameObject audioPrefab;

    // all the audioclips
    [SerializeField] AudioClip playerShoot;                   // sound when the player shoots
    [SerializeField] AudioClip enemyShoot;                    // sound when the enemy shoots
    [SerializeField] AudioClip enemyDeath;                    // sound when the eney dies
    [SerializeField] AudioClip shieldDefelect;                // sound when the shield deflects bullet
    [SerializeField] AudioClip laserCharge;                   // sound when laser charges
    [SerializeField] AudioClip laserShoot;                    // sound when laser shoots
    [SerializeField] AudioClip bugRevive;                     // sound when bug revives
    [SerializeField] AudioClip buttomPress;                   // sound of button press
    [SerializeField] AudioClip healAbilty;
    [SerializeField] AudioClip shieldAbility;
    [SerializeField] AudioClip pauseMenu;
    [SerializeField] AudioClip playerDeath;

    public float minPitch;                          // min pitch value
    public float maxPith;                           // max pitch value
    AudioSource source;                             // source for button press sound
    bool playSoundEffects = true;                   // if the player wants sound effects played


    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();
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
            playSoundEffects = data.playSound;
        }
    }

    // All functions get a audiosource. Get a random pitch value and then play the appropriate sound effect 

    public void PlayPlayerShoot(AudioSource newSource) {
        if (!playSoundEffects)
        {
            Debug.Log("play sounds is false");
            return;
        }
        Debug.Log("do sound");

        float newFloat = Random.Range(minPitch, maxPith);
        newSource.pitch = newFloat;

        newSource.PlayOneShot(playerShoot);
    }

    public void PlayButtonSound() {
        if (!playSoundEffects)
            return;

        float newFloat = Random.Range(minPitch, maxPith);
        source.pitch = newFloat;

        source.PlayOneShot(buttomPress);
    }

    public void PlayEnemyShoot(AudioSource newSource) {
        if (!playSoundEffects)
            return;

        float newFloat = Random.Range(minPitch, maxPith);
        newSource.pitch = newFloat;

        newSource.PlayOneShot(enemyShoot);
    }

    public void PlayEnemyDeath(AudioSource newSource) {
        if (!playSoundEffects)
            return;

        float newFloat = Random.Range(minPitch, maxPith);
        newSource.pitch = newFloat;

        //newSource.PlayOneShot(enemyDeath);
    }

    public void PlayRevive(AudioSource newSource) {
        if (!playSoundEffects)
            return;

        float newFloat = Random.Range(minPitch, maxPith);
        newSource.pitch = newFloat;

        newSource.PlayOneShot(bugRevive);
    }

    public void PlayShieldDeflect(AudioSource newSource) {
        if (!playSoundEffects)
            return;

        float newFloat = Random.Range(minPitch, maxPith);
        newSource.pitch = newFloat;

        newSource.PlayOneShot(shieldDefelect);
    }

    public void PlayChargeUpLaser(AudioSource newSource, bool toggle) {
        if (!playSoundEffects)
            return;

        if (toggle)
        {
            float newFloat = Random.Range(minPitch, maxPith);
            newSource.pitch = newFloat;
            newSource.loop = true;

            newSource.PlayOneShot(laserCharge);
            if (newSource.isPlaying) {
                Debug.Log("playing laser charge");
            }
        }
        else {
            newSource.Stop();
            newSource.clip = null;
            newSource.loop = false;
        }
    }

    public void PlayShootLaser(AudioSource newSource, bool toggle) {
        if (!playSoundEffects)
            return;

        if (toggle)
        {
            float newFloat = Random.Range(minPitch, maxPith);
            newSource.pitch = newFloat;
            newSource.loop = true;

            newSource.clip = laserShoot;
            newSource.Play();

            if (newSource.isPlaying)
            {
                Debug.Log("playing laser shoot");
            }
        }
        else {
            newSource.Stop();
            newSource.clip = null;
            newSource.loop = false;
        }
    }

    public void PlayPlayerDeath() {
        float newFloat = Random.Range(minPitch, maxPith);
        AudioSource newSOurce = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newSOurce.pitch = newFloat;
        newSOurce.clip = playerDeath;
        newSOurce.Play();

        StartCoroutine(DestroyAudioPrefab(newSOurce));
    }

    public void PlayRegainHealth ()
    {
        float newFloat = Random.Range(minPitch, maxPith);
        AudioSource newSOurce = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newSOurce.pitch = newFloat;
        newSOurce.clip = healAbilty;
        newSOurce.Play();

        StartCoroutine(DestroyAudioPrefab(newSOurce));
    }

    public void PlaySheildAbility()
    {
        float newFloat = Random.Range(minPitch, maxPith);
        AudioSource newSOurce = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newSOurce.pitch = newFloat;
        newSOurce.clip = shieldAbility;
        newSOurce.Play();

        StartCoroutine(DestroyAudioPrefab(newSOurce));
    }

    public void PlayOpenPauseMenu()
    {
        float newFloat = Random.Range(minPitch, maxPith);
        AudioSource newSOurce = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newSOurce.pitch = newFloat;
        newSOurce.clip = pauseMenu;
        newSOurce.Play();

        StartCoroutine(DestroyAudioPrefab(newSOurce));
    }

    public void SetIfPlayingSounds(bool play) {
        playSoundEffects = play;
    }

    public bool GetIfCanPlay() {
        return playSoundEffects;
    }

    IEnumerator DestroyAudioPrefab(AudioSource s) {
        yield return new WaitForSeconds(s.clip.length + 0.5f);
        Destroy(s.gameObject);
    }
}
