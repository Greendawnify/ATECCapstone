using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIController : MonoBehaviour
{

    public GameObject winPanel, losePanel;                      //the panels that appear when you lose/win
    public PlayerShooting shootScript;                          // ref to the player shooting script
    public GameObject acceptButton;                             // ref to the accept button in win panel
    public GameObject blackPanel, hitPanel, laserPanel;         // the different panel that can beturned on 
    public GameObject setUpUI;                                  // the setup ui

    GameObject playerCam;                                       // ref to the player cam
    AdsController adScript;

    DataCollection data;                                        // ref to the data collection for this level
    bool playerWon = false, playerLost = false;                 // checks if you won or loss
    GameBoard board;                                            // ref to the game board in this level
    AudioManager audioManager;                                  // controls the button sounds for the ui
    PlayerAbilities abilities;                                  // the abilities of the player. So that they can be reset
    MusicManager musicManager;
    PlayerData playerData;
    PlayerHealth health;
    ProgressPanel wPanel, lPanel;
    bool winHasBeenRestarted = false;
    bool loseHasBeenRestarted = false;
    bool showRewardAdButton = false;

    void Start()
    {
        data = GetComponent<DataCollection>();
        playerData = PlayerData.Instance;
        playerCam = playerData.playerCam;
        board = FindObjectOfType<GameBoard>();
        audioManager = AudioManager.Instance;
        abilities = PlayerAbilities.Instance;
        musicManager = MusicManager.Instance;
        health = playerData.playerHealth;
        wPanel = winPanel.GetComponent<ProgressPanel>();
        lPanel = losePanel.GetComponent<ProgressPanel>();
        adScript = GetComponent<AdsController>();
    }

    /// <summary>
    /// When the player is dead. The logic that is called to calculate loss
    /// </summary>
    public void Lost() {
        if (!playerWon)
        {
           
            musicManager.Pause();

            SetLaserScreen(false);
            playerLost = true;

            board.EnqueEnemies();
            board.ResetBoard();
            board.gameObject.SetActive(false);

            data.StopCountdown();
            data.CalculateLoseData();

            
            losePanel.SetActive(true);
            if (loseHasBeenRestarted)
            {
                losePanel.transform.DORestart();
                lPanel.TurnOnText();
            }



            shootScript.enabled = false;
            StartCoroutine(AcceptStatsButton(false));

            abilities.SetHasUsedAbility(false);

            loseHasBeenRestarted = true;
        }
    }

    /// <summary>
    /// when all the enemies are dead. The logic is called to calculate a victory
    /// </summary>
    public void Win() {
        if (!playerLost)
        {
            
            musicManager.Pause();

            SetLaserScreen(false);
            playerWon = true;

            board.ResetBoard();
            board.gameObject.SetActive(false);
            playerCam.GetComponent<Collider>().enabled = false;


            data.StopCountdown();
            bool userDidBetter;
            data.CalculateWinData(abilities, out userDidBetter);
            if (userDidBetter)
            {
                Debug.Log("The user did better than the last time");
                // congratulate the player on doing better than last time?
                wPanel.TurnOnNewHighScore();
            }
            else {
                Debug.Log("The user did not do better than last time");
                // show the button of the ad reward
                wPanel.TurnOnLowerScoreIndicator();
            }

            shootScript.enabled = false;
            StartCoroutine(AcceptStatsButton(true));

            winPanel.SetActive(true);
            if (winHasBeenRestarted)
            {
                winPanel.transform.DORestart();
                wPanel.TurnOnText();
            }

            abilities.SetHasUsedAbility(false);
            winHasBeenRestarted = true;
        }
    }

    IEnumerator AcceptStatsButton(bool isWinPanel) {
        yield return new WaitForSeconds(1.25f);
        ButtonSound();
        if (isWinPanel)
        {
            wPanel.TurnOnProceedingButtons();
        }
        else {
            lPanel.TurnOnProceedingButtons();
        }

    }

    /// <summary>
    /// loads intro level
    /// </summary>
    public void ReturnToLevelSelect() {
        ButtonSound();
        musicManager.StopMusic();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// loads a specific level
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName) {
        ButtonSound();
        musicManager.StopMusic();
        musicManager.PlayStabByUI();
        SceneManager.LoadScene(sceneName);
    }

    public void RestartLevel() {
        ButtonSound();
        musicManager.StopMusic();
        musicManager.PlayStabByUI();
        playerCam.GetComponent<Collider>().enabled = true;

        // reset dataCollection info
        data.ResetValues();


        // reset setup ui
        setUpUI.SetActive(true);
        setUpUI.GetComponent<SetupTest>().ResetUI();

        // reset this UI and turn it off
        lPanel.ResetPanelUI();
        wPanel.ResetPanelUI();
        ResetUI();
        StartCoroutine(RestartAd()); // turns on ad and waits until it is finished showing
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// toggles blank screen
    /// </summary>
    /// <param name="toggle"></param>
    public void SetBlackScreen(bool toggle) {
        blackPanel.SetActive(toggle);
    }

    /// <summary>
    /// togggles hit screen
    /// </summary>
    /// <param name="toggle"></param>
    public void SetHitScreen(bool toggle) {
        hitPanel.SetActive(toggle);
    }

    /// <summary>
    /// toggles laser screen
    /// </summary>
    /// <param name="toggle"></param>
    public void SetLaserScreen(bool toggle) {
        laserPanel.SetActive(toggle);
    }

    public void ButtonSound() {
        audioManager.PlayButtonSound();
    }

    public void ResetUI() {
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        playerWon = false;
        playerLost = false;
        health.ResetPlayer();
    }

    public void RewardAdButtonClick() {
        wPanel.TurnOffLowerScoreIndicator();
    }

    IEnumerator RestartAd() {
        adScript.CallVideoAd();

        while (!adScript.isAdReady("video")) {
            yield return new WaitForSeconds(0.5f);
        }

        while (adScript.IsAdShowing()) {
            yield return new WaitForSeconds(0.5f);
        }
        gameObject.SetActive(false);
    }
}
