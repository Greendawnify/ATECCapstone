using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData 
{
    public float damageTaken;                                   // the damage user takes during a specific level
    public float timer;                                         // the time the user took to finish this level
    public int shots;                                           // the number shots the player shot
    public int stars;                                           // the anout of stars the player has recieved

    public ScoreData(float dT, float t, int s, int st) {
        damageTaken = dT;
        timer = t;
        shots = s;
        stars = st;
    }
}
