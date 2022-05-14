using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeathData 
{
    public int missionClearedCount;                 // largest amount of level cleared without dying
    public int deaths;                              // the amount of times the player has died playing the game

    public DeathData(int clearCount, int death) {
        missionClearedCount = clearCount;
        deaths = death;
    }
}
