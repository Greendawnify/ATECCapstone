using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScoreSaveData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI levelText, damageText, shotsText, timeText, persistenLevelDataText, starsText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePersistenScoreData() {
        SaveScoreSystem.SaveScoreData(int.Parse(levelText.text), float.Parse(damageText.text), 
            float.Parse(timeText.text), int.Parse(shotsText.text), int.Parse(starsText.text));
    }

    public void SavePersistenDeathData_Dead() {
        SaveScoreSystem.UpdateDeathDataWhenDead();
    }

    public void SavePersistenDeathData_Win()
    {
        SaveScoreSystem.UpdateDeathDateWhenWin();
    }

    public void SavePersisitentLevelData()
    {
        SaveScoreSystem.CompletedAnotherLevel(int.Parse(persistenLevelDataText.text));

    }
}
