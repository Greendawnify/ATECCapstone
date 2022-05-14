using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth;                            // the max health of the player
    public Image healthbar;                                 // reference to the health bar 
    public GameObject uiController;                         // reference to the in game ui
    public GameObject playerShield;                         // reference to the player shield
    public Transform laserAimPoint;                         // point where the laser enemy aims
    public TMPro.TextMeshProUGUI healthIndicator;           //indicates how much health the player has

    float health;                                           // value of health that will be changing when hurt
    UIController ui;                                        // uiController script ref
    DataCollection data;                                    // date colleciton script ref
    Collider playerCol;
    bool playerIsDead = false;                              // if the player is dead
    bool setPlayHeartbeat = false;                          // whether the player is below half health

    MusicManager musicManager;                              //ref to music manager to play hert beat
    AudioManager audioManager;                              // ref to the audio manager to play sounds


    void Start()
    {
        health = startingHealth;
        healthIndicator.text = health.ToString("F0");
        ui = uiController.GetComponent<UIController>();
        data = uiController.GetComponent<DataCollection>();
        musicManager = MusicManager.Instance;
        audioManager = AudioManager.Instance;
        playerCol = GetComponent<Collider>();
    }

    // When I get hit by a enemy bullet I lose health
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(Time.deltaTime + " " + other.name);
        if (other.CompareTag("EnemyBullet")) {
            // get the bullet damage
            float dam = other.GetComponent<Bullet>().damage;

            ui.SetHitScreen(true);
            // send value to lose health funtion
            LoseHealth(dam);


            // destroy the bullet
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Building")) {
            // when in side a building turn a screen that tells them to get out
            ui.SetBlackScreen(true);
        }

        if (other.CompareTag("Laser")) {
            // when hit by a laser turn on the screen indicating that
            ui.SetHitScreen(true);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Laser")) {
            // deal damage over time when hit by the laser
            float dam = other.GetComponent<Bullet>().damage;
            LoseHealth(dam * Time.deltaTime);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Building")) {
            // when exiting the building turn off this screen
            ui.SetBlackScreen(false);
        }
    }

    public void LoseHealth(float dam) {
        if (!playerIsDead)
        {
            // subtract from my health
            health -= dam;

            // aplly new health value to the health bar
            healthbar.fillAmount = health / startingHealth;
            healthIndicator.text = health.ToString("F0");

            // add damage data 
            data.AddDamage(dam);

            // check if player is at or belowhalf health. Should only go off once
            if (health <= startingHealth / 2 && !setPlayHeartbeat) {
                setPlayHeartbeat = true;
                musicManager.PlayHeartBeat();
            }

            // if health is beow zero player is dead
            if (health <= 0f)
            {
                health = 0f;

                healthIndicator.text = health.ToString("F0");
                // die
                Debug.Log("Dead Player");

                audioManager.PlayPlayerDeath();
                playerIsDead = true;
                ui.Lost();
            }
        }
    }

    /// <summary>
    /// called from the Heal Player Ability. Gives player full health
    /// </summary>
    public void RegainFullHealth() {
        audioManager.PlayRegainHealth();
        health = startingHealth;
        healthbar.fillAmount = health / startingHealth;
        healthIndicator.text = health.ToString("F0");
        data.AddDamage(-1f); // resets the damage taken value in DataCollection script

        // reset values for the heartbeat track when healed up
        setPlayHeartbeat = false;
        musicManager.StopHeartbeat();
    }

    /// <summary>
    /// called from Sheild Player Ability. Turns on the shield to block enemy bullets
    /// </summary>
    /// <param name="time"></param>
    public void TurnOnShield(float time) {
        audioManager.PlaySheildAbility();
        Collider col = GetComponent<Collider>() ;
        col.enabled = false;

        playerShield.SetActive(true);
        // some code so that player bullets can go thought he player shield
        StartCoroutine(TurnOffShield(col, time));
    }

    public void ToggleLaserHitScreen(bool toggle) {
        ui.SetLaserScreen(toggle);
    }

    /// <summary>
    /// Turns off the collider when the game is paused
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleCollider(bool toggle) {
        playerCol.enabled = toggle;
    }

    public void ResetPlayer() {
        health = startingHealth;
        healthbar.fillAmount = health / startingHealth;
        healthIndicator.text = health.ToString("F0");

        setPlayHeartbeat = false;
        playerIsDead = false;

        playerCol.enabled = true;
        musicManager.StopHeartbeat();
    }

    IEnumerator TurnOffShield(Collider c, float t) {
        yield return new WaitForSeconds(t);
        c.enabled = true;
        playerShield.SetActive(false);
    }
}
