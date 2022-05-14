using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreenMenu : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI shotsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI deathText;
    public TextMeshProUGUI clearedText;


    /// <summary>
    /// updates the score data in the score menu
    /// </summary>
    public void UpdateScorePersistentData()
    {
        ScoreData data = SaveScoreSystem.LoadAverageScoreDateValue();
        if (data != null)
        {
            damageText.text = data.damageTaken.ToString("F0");
            shotsText.text = data.shots.ToString("F0");
            timeText.text = data.timer.ToString("F0");
        }
    }

    /// <summary>
    /// updates the death data in the score menu
    /// </summary>
    public void UpdateDeathScorePersistenData()
    {
        DeathData data = SaveScoreSystem.GetDeathData();
        if (data != null)
        {
            deathText.text = data.deaths.ToString();
            clearedText.text = data.missionClearedCount.ToString();
        }
    }
}
