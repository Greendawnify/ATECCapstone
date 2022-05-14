using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int levelCompleted;                  // the level that I completed
    public int nextLevelReference;              // the next level after the one I have completed

    public LevelData(int newLevel) {
        levelCompleted = newLevel;
        nextLevelReference = newLevel + 1;
    }
}
