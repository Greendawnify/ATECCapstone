using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public GameObject ModalScreen;                                      // the modal screen that appears for the user to accept their level choice
    public TMPro.TextMeshProUGUI missionName;                           // name of the level to load is accepted
    public float delayTime;                                             // the wait time used in the corutines
    public TMPro.TextMeshProUGUI damageText, shotsText, timeText;       // the Score Date text in the Score Menu
    public TMPro.TextMeshProUGUI deathText, clearedText;                // the Death Data text in the Score Menu
    public Button[] levelButtons;                                       // all the buttons for each level

    public void Quit() {
        Application.Quit();
    }

    //wait for a time
    public void Wait(Transform obj) {
        StartCoroutine(IWait(obj));
    }

    // fades an image
    public void Fade(Image image) {
        StartCoroutine(IFade(image));
    }

    IEnumerator IFade(Image im) {
        
        yield return new WaitForSeconds(delayTime);
        im.DOPlay();
        
        im.DOFade(0f, 1f);
    }

    IEnumerator IWait(Transform ob) {
        ob.DORewind(true);
        yield return new WaitForSeconds(delayTime);
        ob.DOPlay();
    }

    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void LoadScene(TMPro.TextMeshProUGUI nameMission) {
        SceneManager.LoadScene(nameMission.text);
    }

    /// <summary>
    /// Turns on the modal screen with the level name
    /// </summary>
    /// <param name="lvlName"></param>
    public void LoadModalScreen(string lvlName) {
        ModalScreen.SetActive(true);
        missionName.text = lvlName
;    }

    /// <summary>
    /// When the decline button is clicked on the modal screen. 
    /// </summary>
    public void DeclineLevel() {
        ModalScreen.SetActive(false);
        for (int i = 0; i < levelButtons.Length; i++) {
            LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
            lButton.ButtonHighlighted(false);
        }

    }

    /// <summary>
    /// updates the score data in the score menu
    /// </summary>
    public void UpdateScorePersistentData() {
        ScoreData data = SaveScoreSystem.LoadAverageScoreDateValue();
        if (data != null) {
            damageText.text = data.damageTaken.ToString("F0");
            shotsText.text = data.shots.ToString("F0");
            timeText.text = data.timer.ToString("F0");
        }
    }

    /// <summary>
    /// updates the death data in the score menu
    /// </summary>
    public void UpdateDeathScorePersistenData() {
        DeathData data = SaveScoreSystem.GetDeathData();
        if (data != null) {
            deathText.text = data.deaths.ToString();
            clearedText.text = data.missionClearedCount.ToString();
        }
    }

    /// <summary>
    /// Updates the level data for the level buttons
    /// </summary>
    public void UpdateLevelPersistentData() {
        LevelData data = SaveScoreSystem.GetLevelData();
        if (data != null)
        {
            int nextHighestLevel = Mathf.Clamp(data.nextLevelReference, 0, 13);
            Debug.Log("We have level daata");
            for (int i = 0; i <= data.nextLevelReference; i++)
            {
                LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
                levelButtons[i].interactable = true;
                lButton.SetButtonInteractable(true);
            }
        }
        else {
            for (int i = 1; i < levelButtons.Length; i++) {
                LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
                levelButtons[i].interactable = false;
                lButton.SetButtonInteractable(false);
            }
        }
    }
}
