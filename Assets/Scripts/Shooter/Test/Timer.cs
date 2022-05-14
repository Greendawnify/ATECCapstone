using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    public float[] timers;
    public Text timerText;

    float currentTime;
    bool countdown = false;

    private void Awake()
    {
        // making a singleton for this script
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (countdown) {
            currentTime -= 1 * Time.deltaTime;
            SetTimerText(currentTime);

            if (currentTime <= 0f) {
                // level is over
                currentTime = 0f;
                countdown = false;
                // check if you beat the level

            }
        }
    }

    public void SetTimerForNextLevel() {


        SetTimerText(currentTime);

    }

    public void StartCountdown() {
        countdown = true;
    }

    void SetTimerText(float timer) {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public float StopTimer() {
        countdown = false;

        return currentTime;
    }


}
