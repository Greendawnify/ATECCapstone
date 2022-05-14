using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerShooting : MonoBehaviour
{
    public GameObject UICanvas;                                 // ref to in game ui object
    public float overheatShotMax;                               // shots needed to overheat the cooldown
    public float resetCooldownTimer;                            // how long it takes for the cooldown to reset
    public Image shootingCooldown;                              // ref to the image that indicates the cooldown
    public Image cooldownMask;                                  // the image on top of the cool down ui
    public string spawnFromPoolTag;                             // the pool the bullets are spawned from
    public GameObject pauseMenu;


    ARSessionOrigin arOrigin;                                   // reference to ar session origin
    Camera camera;                                              // reference to ar camera
    AudioSource source;                                         // reference the audio source
    DataCollection data;                                        // ref to player data
    bool restoreCooldown = false;                               // if it is time to restore the cool down
    [SerializeField] bool canShoot = false;                                       // if the player can shoot
    bool pauseMenuIsOn = false;
    bool twoTouch = false;
    float timer = 1.5f;
    bool resetCourutineStarted = false;
    ObjectPooler pooler;                                        // ref to the ObjectPooler
    MusicManager musicManager;
    PlayerHealth health;
    AudioManager audioManager;
    GameBoard gameBoard;

    private void OnEnable()
    {
        camera = GetComponent<Camera>();
        source = GetComponent<AudioSource>();
        data = UICanvas.GetComponent<DataCollection>();
        pooler = ObjectPooler.Instance;
        musicManager = MusicManager.Instance;
        health = GetComponent<PlayerHealth>();
        audioManager = AudioManager.Instance;
    }

    void Update()
    {
          // if you touch the screen or click it with mouse
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            if (Input.touchCount > 0)
            {
                // send touch position info if I touched the screen
                DoRaycast(Input.GetTouch(0).position);
            }
            else
            {
                // send mouse position info if I clicked
                DoRaycast(Input.mousePosition);
            }

        }

        // two touches
        if ((Input.touchCount == 2) || (Input.GetKeyDown(KeyCode.P)))
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Stationary && touch2.phase == TouchPhase.Stationary) {
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    if (!pauseMenuIsOn)
                    {
                        // call pause game code
                        pauseMenuIsOn = true;
                        Pause();

                    }
                }
            }

        }
        

        // means we are shooting reguraly and have not overheated
        if (restoreCooldown && canShoot) {
            shootingCooldown.fillAmount += 0.075f * Time.deltaTime;
            
        }
    }

    void DoRaycast(Vector2 touch) {
        if (canShoot)
        {
            // creates a ray based on where the screen is interacted with
            Ray ray = camera.ScreenPointToRay(new Vector3(touch.x, touch.y, 0f));

            // creates a player bullet and calls a funtion in PlayerBullet script
            //GameObject newObj = Instantiate(bullet, ray.origin, Quaternion.identity);
            GameObject newObj = pooler.SpawnFromPool(spawnFromPoolTag, ray.origin, Quaternion.identity);
            newObj.SetActive(true);
            newObj.GetComponent<PlayerBullet>().SetBulletShooting(ray.direction);

            if (source)
                AudioManager.Instance.PlayPlayerShoot(source);

            data.AddNumberOfShots();
            ChangeCooldown();

            if(!resetCourutineStarted)
                StartCoroutine(ResetCooldown());
        }
    }

    /// <summary>
    /// change the cool down image value. If runs out playe cant shoot
    /// </summary>
    void ChangeCooldown() {
        shootingCooldown.fillAmount -= 1 / overheatShotMax;
        restoreCooldown = true;

        if (shootingCooldown.fillAmount <= 0) {
            StopAllCoroutines();
            shootingCooldown.fillAmount = 0;
            restoreCooldown = false;
            canShoot = false;
            cooldownMask.DOColor(Color.red, 0.5f).OnComplete(MaskDotweenRestart);
            StartCoroutine(ResetCooldown());
        }
    }

    void MaskDotweenRestart() {
        cooldownMask.DORestart();
    }

    void Pause() {
        canShoot = false;
        twoTouch = false;
        timer = 1.5f;

        audioManager.PlayButtonSound();
        pauseMenu.SetActive(true);
        musicManager.Pause();
        health.ToggleCollider(false);
    }

    public void QuitLevel(bool quit) {
        if (quit)
        {
            // exit to the main menu
            musicManager.StopMusic();
            audioManager.PlayButtonSound();
            gameBoard = FindObjectOfType<GameBoard>();
            gameBoard.EnqueEnemies();
            
            SceneManager.LoadScene("MainMenu");

        }
        else {
            // unpause game

            pauseMenu.SetActive(false);
            pauseMenuIsOn = false;
            musicManager.Pause();
            health.ToggleCollider(true);
            canShoot = true;
        }
    }

    public void SetCanShoot(bool set) {
        canShoot = set;
    }

    IEnumerator ResetCooldown() {
        resetCourutineStarted = true;
        yield return new WaitForSeconds(resetCooldownTimer);
        shootingCooldown.fillAmount = 1f;
        canShoot = true;
        resetCourutineStarted = false;
        cooldownMask.DORewind();
        cooldownMask.DOPause();
    }
}
