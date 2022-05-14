using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public GameObject lockedIcon;                       // the lock image
    public GameObject infoPanel;                        // panel that holds all the info on it like number and stars
    public GameObject highlightedBackgorund;            // the object that highlights the level button when selected
    public GameObject[] stars;                          // the stars in the level button
    public bool isHighlighted = false;                  // if the button is highlighted
    public int levelReference = -1;                     // the refereence each button has. Corresponds to DataColleciton
    public Collider col;
    public string sceneName;                            // name of the scene this level loads
    public bool firstUX = false;                        // tells about normal enemies and abilities
    public bool secondUX = false;                       // tells about swarm enemies
    public bool thirdUX = false;                        // tells abouyt laser enmuies
    public bool fourthUX = false;                       // tells about shields

    public LevelSelectMenuInteraction menu;             // ref to the menu controller

    private void Start()
    {
        col = GetComponent<Collider>();
        //menu = FindObjectOfType<LevelSelectMenuInteraction>();
    }



    /// <summary>
    /// If this level button should be unlocked. Based on saved Score Data
    /// </summary>
    /// <param name="toggle"></param>
    public void SetButtonInteractable(bool toggle) {
        if (toggle)
        {
            lockedIcon.SetActive(false);
            infoPanel.SetActive(true);
            if(col)
                col.enabled = true;

            ScoreData data = SaveScoreSystem.LoadSpecifcLevelScoreData(levelReference);
            if (data != null)
            {
                for (int i = 0; i < data.stars; i++)
                {
                    stars[i].SetActive(true);
                }
            }
            /*else {
                lockedIcon.SetActive(true);
                infoPanel.SetActive(false);
            }*/
        }
        else {
            lockedIcon.SetActive(true);
            infoPanel.SetActive(false);
            if (col)
                col.enabled = false;
        }
    }

    /// <summary>
    /// toggles if the button is highlighted
    /// </summary>
    /// <param name="toggle"></param>
    public void ButtonHighlighted(bool toggle) {
        isHighlighted = toggle;
        highlightedBackgorund.SetActive(toggle);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) {
            Debug.Log("selected a lvel " + sceneName);

            ButtonHighlighted(true);
            menu.LoadModalScreen(sceneName);
        }
    }
}
