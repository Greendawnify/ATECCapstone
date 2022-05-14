using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectMenuInteraction : MonoBehaviour
{
    public GameObject modalScren;
    public TextMeshProUGUI missionName;
    public Button[] levelButtons;
    public CallBannerAd bannerAdScript;
    public MenuShooter shootingScript;

    public void LoadScene(TextMeshProUGUI nameMission)
    {
        bannerAdScript.DisableBannerAd();
        SceneManager.LoadScene(nameMission.text);
    }

    /// <summary>
    /// Turns on the modal screen with the level name
    /// </summary>
    /// <param name="lvlName"></param>
    public void LoadModalScreen(string lvlName)
    {
        shootingScript.enabled = false;
        modalScren.SetActive(true);
        if (missionName.text == "")
        {
            missionName.text = lvlName;
            /*for (int i = 0; i < levelButtons.Length; i++)
            {
                LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
                lButton.col.enabled = false;
            }*/
        }
;
    }

    /// <summary>
    /// When the decline button is clicked on the modal screen. 
    /// </summary>
    public void DeclineLevel()
    {
        modalScren.SetActive(false);
        missionName.text = "";
        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
            lButton.ButtonHighlighted(false);
        }
        shootingScript.enabled = true;
    }

    public void UpdateLevelPersistentData()
    {
        LevelData data = SaveScoreSystem.GetLevelData();
        if (data != null)
        {
            int highestLevelReference = Mathf.Clamp(data.nextLevelReference, 0, levelButtons.Length - 1);

            Debug.Log("We have level daata for "+ data.nextLevelReference);
            for (int i = 0; i <= highestLevelReference; i++)
            {
                LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
                levelButtons[i].interactable = true;
                lButton.SetButtonInteractable(true);
            }
        }
        else
        {
            for (int i = 1; i < levelButtons.Length; i++)
            {
                LevelButton lButton = levelButtons[i].GetComponent<LevelButton>();
                levelButtons[i].interactable = false;
                lButton.SetButtonInteractable(false);
            }
        }
    }
}
