using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

// collects data on how well the player is doing
public class DataCollection : MonoBehaviour
{
    public int levelReference;                                              // reference to the level this data is being collected for
    public int maxPointThreshold;                                           // the max points you can achive in this level. Decided how many stars you get
    public int maxShotsRequired;                                            // the min shots you need to kill all the enemies
    public int threeStarScore;                                              // score you need in this level to get 3 star
    public int twoStarScore;                                                // score you need in this level to get 2 star
    public TextMeshProUGUI[] shotsText;                                     // refs to text to display number of shots
    public TextMeshProUGUI[] damageText;                                    // refs to text to display damage taken
    public TextMeshProUGUI[] timeText;                                      // refs to text to display time it took to finish
    public GameObject[] stars;                                              // the stars that show how well you did in the level
    public GameObject previousScorePanel;                                   // the panel that has the pervious score info
    public TextMeshProUGUI previousShot, previousDamage, previousTime;      // damage, time, shot text so I can display the previous score info



    bool countdown = false;                                                 // tell if we should be counting time for the time score
    float timer = 0f;                                                       // the timer value that is calculated
    int numShots = 0;                                                       // number of shots shot by the player
    float damageTaken = 0f;                                                 // the damage taken by the player
    int starsAquired = 0;                                                   // the amount of stars you acjeived for this level. max is 3
    ScoreData previousData;
    void Start()
    {
        countdown = true;   
    }

    private void OnEnable()
    {
        countdown = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown) {
            timer += 1f * Time.deltaTime;
        }
    }
    /// <summary>
    /// Called when the player wins a level. Calculates all the data for this specifc level
    /// </summary>
    public void CalculateWinData(PlayerAbilities ability, out bool userDidBetter) {

        SetText();

        // geting previos data
        previousData = SaveScoreSystem.LoadSpecifcLevelScoreData(this);
        userDidBetter = ComparePreviousScores(previousData);

        SaveScoreSystem.UpdateDeathDateWhenWin();
        SaveScoreSystem.CompletedAnotherLevel(levelReference);
        
        // calculating the score for this level
        int shotScore = Mathf.Abs(numShots - maxShotsRequired);
        // 100 - 3(shotScore) - 2(damageTaken) - (int)timerInMinutes
        int finalScore = maxPointThreshold - (3 * shotScore) - (2 * (int)damageTaken);
        if (!ability.GetHasUsedAbility()) // if the player has not used their ability they get a boost in their score for the level
            finalScore += 10;

        Debug.Log("Final score " + finalScore);
        float previousScoreDataWait  =0f;

        // based on the final score you get 1- 3 stars
        if (finalScore >= threeStarScore)
        {
            // 3 stars go up
            StartCoroutine(StarTime(1f, stars[0]));
            StartCoroutine(StarTime(1.25f, stars[1]));
            StartCoroutine(StarTime(1.5f, stars[2]));
            previousScoreDataWait = 2.5f;
            starsAquired = 3;
        }
        else if (finalScore >= twoStarScore)
        {
            // 2 stars go up
            StartCoroutine(StarTime(1f, stars[0]));
            StartCoroutine(StarTime(1.25f, stars[1]));
            previousScoreDataWait = 2.25f;
            starsAquired = 2;
        }
        else if(finalScore >= 1)
        {
            // one star goes up
            StartCoroutine(StarTime(1f, stars[0]));
            previousScoreDataWait = 2f;
            starsAquired = 1;
        }
        // saving the data
        SaveScoreSystem.SaveScoreData(this);
        StartCoroutine(TurnOnPreviousScoreData(previousScoreDataWait, previousData));
    }

    /// <summary>
    /// Calculate the data when the player loses a level
    /// </summary>
    public void CalculateLoseData() {
        SetText();

        // saves data of this level
        SaveScoreSystem.UpdateDeathDataWhenDead();
        SaveScoreSystem.SaveScoreData(this);
    }

    /// <summary>
    /// Applies the score values and turns them into strings to show in panels
    /// </summary>
    void SetText() {
        int newInt = (int)timer;
        for (int i = 0; i < timeText.Length; i++) {
            timeText[i].text = newInt.ToString("F0");
        }

        for (int i = 0; i < shotsText.Length; i++)
        {
            shotsText[i].text = numShots.ToString("F0");
        }

        for (int i = 0; i < damageText.Length; i++)
        {
            damageText[i].text = damageTaken.ToString("F0");
        }
    }

    public void ResetValues() {
         countdown = false;
         timer = 0f;
         numShots = 0; 
         damageTaken = 0f;
         starsAquired = 0;
    }

    public void RewardResetScoreData() {
        // reset the score data for this level to what it was previously so that the user does not get a worse average
        // make a function in SaveScoreSystem that takes a specific level and specific data for a score and resets what it was to this new one
        // since each DataCollection is unique per level should be able to save the Previous ScoreData, then when reward ad is clicked
        // feed that info into this new function I will create in SaveScoreSystem

        SaveScoreSystem.SaveScoreData(levelReference, previousData.damageTaken, previousData.timer, previousData.shots, previousData.stars);
        previousDamage.text = previousData.damageTaken.ToString("F0") + "   UPDATED";
        previousShot.text = previousData.shots.ToString("F0") + "   UPDATED";
        previousTime.text = previousData.timer.ToString("F0") + "   UPDATED";
    }

    bool ComparePreviousScores(ScoreData previousData) {
        if (previousData == null)
            return true;

        if (previousData.damageTaken < damageTaken ||
            previousData.shots < numShots ||
            previousData.timer < timer) {
            // the previous score for this level was better is one or more aspects
            return false;
        }

        return true;
    }

    IEnumerator StarTime(float wait, GameObject s) {
        yield return new WaitForSeconds(wait);
        s.SetActive(true);
        s.transform.DORestart();
    }

    /// <summary>
    /// Applies the previous data info and puts it on the string panels
    /// </summary>
    /// <param name="wait"></param> how long to wait for
    /// <param name="data"></param> the previous score data info
    /// <returns></returns>
    IEnumerator TurnOnPreviousScoreData(float wait, ScoreData data) {
        yield return new WaitForSeconds(wait);
        previousScorePanel.SetActive(true);

        if (data != null)
        {
            previousDamage.text = data.damageTaken.ToString("F0");
            previousShot.text = data.shots.ToString();
            previousTime.text = data.timer.ToString("F0");
        }
        else {
            previousDamage.text = "N/A";
            previousShot.text = "N/A";
            previousTime.text = "N/A";
        }
    }

    public void StopCountdown() {
        countdown = false;

    }

    public void AddNumberOfShots() {
        numShots++;
    }

    public void AddDamage(float damage) {
        if (damage < 0f)
        { // only happens when the player fully regains health
            damageTaken = 0;
        }
        else {
            damageTaken += damage;
        }
    }

    public float GetTimer() {
        return timer;
    }

    public float GetDamageTaken() {
        return damageTaken;
    }

    public int GetNumberShots() {
        return numShots;
    }

    public int GetStarsAquired() {
        return starsAquired;
    }
}
