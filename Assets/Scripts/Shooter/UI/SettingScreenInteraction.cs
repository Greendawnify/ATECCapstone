using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingScreenInteraction : MonoBehaviour
{
    public string tag;                                  // the id for each trigger that has this script
    public Animator menuBoardAnim;                      // the main menu baord that goes away when triggered
    public Animator otherBoardAnimator;                 // the other board that comes in when this is triggered
    public GameObject thisThumb, otherThumb;            // ref to the image indicators for the buttons
    public GameObject mainMenuTimeLine;

    MusicManager musicManager;
    AudioManager audioManager;

    private void Start()
    {
        musicManager = MusicManager.Instance;
        audioManager = AudioManager.Instance;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            MenuAction();
        }
    }

    void MenuAction()
    {
        switch (tag)
        {
            case "exit":
                Debug.Log("exit trigger");
                menuBoardAnim.SetTrigger("exit");
                otherBoardAnimator.SetTrigger("enter");
                StartCoroutine(wait());
                break;
            case "yes":
                otherThumb.SetActive(false);
                OptionData data = SaveScoreSystem.GetOptionData();

                if (data == null)
                {
                    SaveScoreSystem.SetUXOptionData(null, true);
                }
                else {
                    SaveScoreSystem.SetUXOptionData(data, true);
                }
                thisThumb.SetActive(true);
                break;
            case "no":
                otherThumb.SetActive(false);
                OptionData _data = SaveScoreSystem.GetOptionData();

                if (_data == null)
                {
                    SaveScoreSystem.SetUXOptionData(null, false);
                }
                else
                {
                    SaveScoreSystem.SetUXOptionData(_data, false);
                }
                thisThumb.SetActive(true);

                break;
            case "PlayMusic":
                otherThumb.SetActive(false);
                musicManager.SetCanPlayMusic(true);
                mainMenuTimeLine.GetComponent<PlayMainMenuMusic>().SetPlay(true);

                OptionData musicData = SaveScoreSystem.GetOptionData();

                if (musicData == null)
                {
                    SaveScoreSystem.SetMusicOptionData(null, true);
                }
                else
                {
                    SaveScoreSystem.SetMusicOptionData(musicData, true);
                }

                thisThumb.SetActive(true);
                break;
            case "NoMusic":
                otherThumb.SetActive(false);
                musicManager.SetCanPlayMusic(false);
                mainMenuTimeLine.GetComponent<PlayMainMenuMusic>().SetPlay(false);

                OptionData _musicData = SaveScoreSystem.GetOptionData();

                if (_musicData == null)
                {
                    SaveScoreSystem.SetMusicOptionData(null, false);
                }
                else
                {
                    SaveScoreSystem.SetMusicOptionData(_musicData, false);
                }

                thisThumb.SetActive(true);
                break;
            case "PlaySound":
                otherThumb.SetActive(false);
                audioManager.SetIfPlayingSounds(true);

                OptionData soundData = SaveScoreSystem.GetOptionData();

                if (soundData == null)
                {
                    SaveScoreSystem.SetSoundOptionData(null, true);
                }
                else
                {
                    SaveScoreSystem.SetSoundOptionData(soundData, true);
                }

                thisThumb.SetActive(true);
                break;
            case "NoSound":
                otherThumb.SetActive(false);
                audioManager.SetIfPlayingSounds(false);

                OptionData _soundData = SaveScoreSystem.GetOptionData();

                if (_soundData == null)
                {
                    SaveScoreSystem.SetSoundOptionData(null, false);
                }
                else
                {
                    SaveScoreSystem.SetSoundOptionData(_soundData, false);
                }

                thisThumb.SetActive(true);
                break;
        }
    }

    IEnumerator wait()
    {
        Debug.Log("wait called");
        yield return new WaitForSeconds(1f);
        otherBoardAnimator.SetTrigger("enter");
    }
}
