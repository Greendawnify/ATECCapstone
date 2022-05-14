using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DG.DemiLib;

public class ProgressPanel : MonoBehaviour
{
    public bool isWinPanel;
    [Header("Buttons")]
    public Button restart;
    public Button exit;
    public Button nextLevel;
    [Header("Objects")]
    public GameObject[] stars;
    public GameObject previousData;
    public GameObject highscoreInidcator;
    public GameObject rewardAdButton;
    [Header("Text")]
    public TextMeshProUGUI numShots;
    public TextMeshProUGUI damageTaken;
    public TextMeshProUGUI timer;

    Transform thisPanel;

    // Start is called before the first frame update
    void Start()
    {
        thisPanel = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPanelUI() {
        restart.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);

        numShots.transform.DORewind();
        numShots.gameObject.SetActive(false);

        damageTaken.transform.DORewind();
        damageTaken.gameObject.SetActive(false);

        timer.transform.DORewind();
        timer.gameObject.SetActive(false);

        if (isWinPanel)
        {
            //Resetting the win panel
            nextLevel.gameObject.SetActive(false);
            previousData.SetActive(false);

            highscoreInidcator.transform.DORewind();
            highscoreInidcator.SetActive(false);

            rewardAdButton.transform.DORewind();
            rewardAdButton.SetActive(false);

            for (int i = 0; i < stars.Length; i++) {
                stars[i].transform.DORewind();
                stars[i].SetActive(false);
            }
            
        }
    }

    public void TurnOnNewHighScore() {
        highscoreInidcator.SetActive(true);
        highscoreInidcator.transform.DOPlay();
    }

    public void TurnOnLowerScoreIndicator() {
        rewardAdButton.SetActive(true);
        rewardAdButton.transform.DOPlay();
    }

    public void TurnOffLowerScoreIndicator() {
        rewardAdButton.SetActive(false);
        rewardAdButton.transform.DORestart();
        
    }

    public void TurnOnText() {
        numShots.gameObject.SetActive(true);
        numShots.transform.DORestart();

        damageTaken.gameObject.SetActive(true);
        damageTaken.transform.DORestart();

        timer.gameObject.SetActive(true);
        timer.transform.DORestart();
    }

    public void TurnOnProceedingButtons() {
        restart.gameObject.SetActive(true);
        exit.gameObject.SetActive(true);

        if(nextLevel != null)
            nextLevel.gameObject.SetActive(true);
    }
}
