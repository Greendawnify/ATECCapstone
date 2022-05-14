using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    public GameObject yesIndicator, noIndicator, playMusicIndicator, noMusicIndicator;
    public GameObject playSoundIndicator, noSoundIndicator;
    public PlayMainMenuMusic musicPlayer;

    MusicManager music;
    AudioManager audio;

    // Start is called before the first frame update
    void Start()
    {
        music = MusicManager.Instance;
        audio = AudioManager.Instance;
    }

    private void OnEnable()
    {
        // check Option data when ever turned on
        OptionData data = SaveScoreSystem.GetOptionData();

        SaveScoreSystem.SetUXOptionData(data, true);

        data = SaveScoreSystem.GetOptionData();

        if (music == null)
            music = MusicManager.Instance;

        if (audio == null)
            audio = AudioManager.Instance;

        StartCoroutine( InitialSetup(data));
    }


    IEnumerator InitialSetup(OptionData data) {
        yield return new WaitForSeconds(0.5f);
        if (data.playSound)
        {
            playSoundIndicator.SetActive(true);
            noSoundIndicator.SetActive(false);

          
            audio.SetIfPlayingSounds(true);

        }
        else {
            playSoundIndicator.SetActive(false);
            noSoundIndicator.SetActive(true);

            audio.SetIfPlayingSounds(false);
        }

        if (data.showPregameInfo)
        {
            yesIndicator.SetActive(true);
            noIndicator.SetActive(false);

        }
        else {
            yesIndicator.SetActive(false);
            noIndicator.SetActive(true);
        }

        if (data.playMusic)
        {
            playMusicIndicator.SetActive(true);
            noMusicIndicator.SetActive(false);

            music.SetCanPlayMusic(true);
            musicPlayer.SetPlay(true);
        }
        else {
            playMusicIndicator.SetActive(false);
            noMusicIndicator.SetActive(true);

            music.SetCanPlayMusic(false);
            musicPlayer.SetPlay(false);
        }
    }
}
